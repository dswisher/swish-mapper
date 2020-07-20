
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class EmptyEntityCleaner : IEmptyEntityCleaner
    {
        private readonly ILogger logger;

        public EmptyEntityCleaner(ILogger<EmptyEntityCleaner> logger)
        {
            this.logger = logger;
        }


        public IModelProducer Input { get; set; }


        public async Task<DataModel> RunAsync()
        {
            var model = await Input.RunAsync();

            // Determine which entities we're going to purge
            var doomedEntities = model.Entities.Where(x => x.Attributes.Count == 0);

            // Fix up any references to the doomed entities
            foreach (var sadEntity in doomedEntities)
            {
                // Note that the ReferencedBy field is not set until the model is complete, so we
                // cannot leverage it here.
                var referers = model.Entities
                    .SelectMany(x => x.Attributes)
                    .Where(x => x.DataType != null)
                    .Where(x => (x.DataType.Type == PrimitiveType.Ref) && (x.DataType.RefName == sadEntity.Name));

                foreach (var referer in referers)
                {
                    referer.DataType = sadEntity.DataType;
                }
            }

            // Remove the doomed entities from the model
            foreach (var entity in doomedEntities)
            {
                model.DestroyEntity(entity);
            }

            // Return the result
            return model;
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
