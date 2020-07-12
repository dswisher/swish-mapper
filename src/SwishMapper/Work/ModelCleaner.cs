
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class ModelCleaner : IModelProducer
    {
        private readonly ILogger logger;

        public ModelCleaner(ILogger<ModelCleaner> logger)
        {
            this.logger = logger;
        }


        public IModelProducer Input { get; set; }


        public async Task<DataModel> RunAsync()
        {
            var model = await Input.RunAsync();

            // TODO - xyzed - implement me!
            logger.LogWarning("ModelCleaner is not yet implemented!");

            return model;   // TODO - HACK!
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
