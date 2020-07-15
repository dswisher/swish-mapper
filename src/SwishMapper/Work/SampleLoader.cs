
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class SampleLoader : IModelProducer
    {
        private readonly ILogger logger;

        public SampleLoader(ILogger<SampleLoader> logger)
        {
            this.logger = logger;
        }


        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string InputPath { get; set; }
        public string SampleId { get; set; }
        public SampleWriter Writer { get; set; }


        public async Task<DataModel> RunAsync()
        {
            // Have the writer do its thing, making sure we have an up-to-date sample file.
            await Writer.RunAsync();

            // Load the sample file
            SampleJson content;
            using (var stream = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            {
                content = await JsonSerializer.DeserializeAsync<SampleJson>(stream);

                logger.LogDebug("Loaded samples from {Name}, found {Num1} filenames and {Num2} data points.",
                        InputPath, content.Filenames.Count, content.DataPoints.Count);
            }

            // Create a model from the sample file
            var model = new DataModel
            {
                Id = ModelId,
                Name = ModelName
            };

            var source = new DataModelSource
            {
                ShortName = SampleId,
                Path = InputPath
            };

            model.Sources.Add(source);

            foreach (var dataPoint in content.DataPoints)
            {
                var bits = dataPoint.Path.Split('/');
                if (bits.Length < 2)
                {
                    throw new LoaderException($"Sample {dataPoint.Path} has too few segments!");
                }

                var entityName = bits[bits.Length - 2];
                var attributeName = bits[bits.Length - 1];

                if (attributeName.StartsWith("@"))
                {
                    attributeName = attributeName.Substring(1);
                }

                var entity = model.FindOrCreateEntity(entityName, source);
                var attribute = entity.FindOrCreateAttribute(attributeName, source);

                attribute.Samples.Add(new DataAttributeSample
                {
                    SampleId = SampleId,
                    Path = dataPoint.Path,
                    Top5 = dataPoint.Samples.Select(x => x.Value)
                });
            }

            logger.LogWarning("SampleLoader - still a WIP!");

            return model;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this);

            using (var childContext = context.Push())
            {
                Writer.Dump(childContext);
            }
        }
    }
}
