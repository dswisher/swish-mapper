
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class ModelMerger : IModelMerger
    {
        private readonly IAttributeMerger attributeMerger;
        private readonly ILogger logger;

        public ModelMerger(IAttributeMerger attributeMerger,
                           ILogger<ModelMerger> logger)
        {
            this.attributeMerger = attributeMerger;
            this.logger = logger;
        }


        public IModelProducer Input { get; set; }


        public async Task RunAsync(DataModel targetModel)
        {
            // Get the model from the mutator...
            var sourceModel = await Input.RunAsync();

            // Merge the sources
            foreach (var source in sourceModel.Sources)
            {
                targetModel.Sources.Add(source);
            }

            // Merge the entities
            foreach (var sourceEntity in sourceModel.Entities)
            {
                var targetEntity = targetModel.FindOrCreateEntity(sourceEntity.Name, sourceEntity.Sources);

                MergeProperty(targetEntity, sourceEntity, "Comment", x => x.Comment, (x, y) => x.Comment = y);

                // Merge the attributes
                foreach (var sourceAttribute in sourceEntity.Attributes)
                {
                    var targetAttribute = targetEntity.FindOrCreateAttribute(sourceAttribute.Name, sourceAttribute.Sources);

                    attributeMerger.Merge(targetAttribute, sourceAttribute);
                }
            }
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this);

            using (var childContext = context.Push())
            {
                Input.Dump(childContext);
            }
        }


        private void MergeProperty(DataEntity targetEntity, DataEntity sourceEntity, string propertyName,
                Func<DataEntity, string> getter, Action<DataEntity, string> setter)
        {
            var source = getter(sourceEntity);
            var target = getter(targetEntity);

            // If we don't have data, we're done.
            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            // If the source or target is empty, we can't have a conflict
            if (string.IsNullOrEmpty(target))
            {
                setter(targetEntity, source);
            }
            else if (source != target)
            {
                var modelId = sourceEntity.Parent.Id;

                var sourceSource = sourceEntity.Sources.First()?.ShortName;

                logger.LogWarning("Merge conflict from {Source} on {Model}, {Entity}, {PropName} has current value '{Old}', proposed value is '{New}'.",
                        sourceSource, modelId, sourceEntity.Name, propertyName, target, source);
            }
        }
    }
}
