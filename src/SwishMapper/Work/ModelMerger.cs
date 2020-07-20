
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class ModelMerger : IModelMerger
    {
        private readonly IEntityMerger entityMerger;
        private readonly IAttributeMerger attributeMerger;
        private readonly ILogger logger;

        public ModelMerger(IEntityMerger entityMerger,
                           IAttributeMerger attributeMerger,
                           ILogger<ModelMerger> logger)
        {
            this.entityMerger = entityMerger;
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

                // Merge the entity properties
                entityMerger.Merge(targetEntity, sourceEntity);

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
    }
}
