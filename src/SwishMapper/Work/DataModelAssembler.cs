
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class DataModelAssembler : IWorker<DataModel>
    {
        private readonly List<IPopulator> populators = new List<IPopulator>();

        private readonly ILogger logger;

        public DataModelAssembler(ILogger<DataModelAssembler> logger)
        {
            this.logger = logger;
        }

        public string Name { get; set; }
        public string Id { get; set; }

        public IList<IPopulator> Populators { get { return populators; } }


        public async Task<DataModel> RunAsync()
        {
            // Create the data model that we'll populate...
            var model = new DataModel();

            // Go through all the populators, and have 'em do the needful.
            // TODO - do we care at all about ordering?
            foreach (var pop in populators)
            {
                await pop.RunAsync(model);
            }

            logger.LogDebug("DataModelAssembler, {Name}, entity count = {Num}.", Name, model.Entities.Count());

            // Return what we've built
            return model;
        }
    }
}
