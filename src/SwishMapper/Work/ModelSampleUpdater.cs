
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class ModelSampleUpdater : IModelSampleUpdater
    {
        private readonly ILogger logger;


        public ModelSampleUpdater(ILogger<ModelSampleUpdater> logger)
        {
            this.logger = logger;
        }


        public SampleWriter Writer { get; set; }
        public string SampleId { get; set; }


        public async Task RunAsync(DataModel model)
        {
            // Have the writer do its thing, making sure we have up-to-date samples.
            var content = await Writer.RunAsync();

            // Go through all the data points, and process 'em.
            foreach (var dataPoint in content.DataPoints)
            {
                // Pick apart the path
                var bits = dataPoint.Path.Split('/');
                if (bits.Length < 2)
                {
                    throw new LoaderException($"Sample {dataPoint.Path} has too few segments!");
                }

                // Work our way down the path. To start, find the top-most entity.
                var entity = model.FindEntity(bits.First());

                if (entity == null)
                {
                    logger.LogWarning("Could not find top-level entity '{Name}' for sample path {Path}.", bits.First(), dataPoint.Path);
                    continue;
                }

                var sample = new DataAttributeSample
                {
                    SampleId = SampleId,
                    Path = dataPoint.Path,
                    Top5 = dataPoint.Samples.Select(x => x.Value).Take(5)
                };

                ProcessEntity(model, entity, sample, bits.Skip(1));
            }
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this);
        }


        private void ProcessEntity(DataModel model, DataEntity entity, DataAttributeSample sample, IEnumerable<string> remainingPath)
        {
            var attributeName = remainingPath.First();
            if (attributeName.StartsWith("@"))
            {
                attributeName = attributeName.Substring(1);
            }

            // If we're at the last part of the path, add the sample...
            if (remainingPath.Count() == 1)
            {
                var attribute = entity.FindAttribute(attributeName);
                if (attribute == null)
                {
                    entity.OutcastSamples.Add(sample);
                    logger.LogWarning("{Model}: Could not find attribute '{Attribute}' in {Entity} for sample path {Path}.",
                            model.Id, attributeName, entity.Name, sample.Path);
                }
                else
                {
                    attribute.Samples.Add(sample);
                }
            }
            else
            {
                // Not at the last part, find the attribute within this entity and recurse down...
                var childAttribute = entity.FindAttribute(attributeName);

                if (childAttribute == null)
                {
                    entity.OutcastSamples.Add(sample);
                    logger.LogWarning("{Model}: Could not find intermediate attribute '{Attribute}' in {Entity} for sample path {Path}.",
                            model.Id, entity.Name, attributeName, sample.Path);
                    return;
                }

                // The childAttribute should be a ref type. If not, we're in trouble.
                if (childAttribute.DataType.Type != PrimitiveType.Ref)
                {
                    entity.OutcastSamples.Add(sample);
                    logger.LogWarning("{Model}: Expected attribute {Entity}.{Attribute} to be type 'ref' for sample path {Path}.",
                            model.Id, entity.Name, attributeName, sample.Path);
                    return;
                }

                var childEntity = model.FindEntity(childAttribute.DataType.RefName);

                if (childEntity == null)
                {
                    entity.OutcastSamples.Add(sample);
                    logger.LogWarning("{Model}: Could not find child ref '{Name}' for {Entity}.{Attribute} when searching sample path {Path}.",
                            model.Id, childAttribute.DataType.RefName, entity.Name, attributeName, sample.Path);
                    return;
                }

                ProcessEntity(model, childEntity, sample, remainingPath.Skip(1));
            }
        }
    }
}
