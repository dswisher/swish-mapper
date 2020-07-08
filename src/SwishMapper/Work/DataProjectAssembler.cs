
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class DataProjectAssembler : IWorker<DataProject>
    {
        private readonly List<DataModelAssembler> modelWorkers = new List<DataModelAssembler>();

        private readonly ILogger logger;

        public DataProjectAssembler(ILogger<DataProjectAssembler> logger)
        {
            this.logger = logger;
        }


        public async Task<DataProject> RunAsync()
        {
            logger.LogDebug("Assembling DataProject...");

            // Create the project
            var project = new DataProject();

            // Build all the models, and add them to the project
            foreach (var model in await Task.WhenAll(modelWorkers.Select(x => x.RunAsync())))
            {
                project.Models.Add(model);
            }

            // Process all the mappings, and get them into the project, too.
            // TODO - process mappings

            // Return the resulting project that we've built
            return project;
        }


        public void AddWorker(DataModelAssembler worker)
        {
            modelWorkers.Add(worker);
        }
    }
}
