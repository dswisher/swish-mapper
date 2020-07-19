
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class DataModelAssembler : IWorker<DataModel>
    {
        private readonly List<IModelMerger> mergers = new List<IModelMerger>();
        private readonly List<IModelUpdater> updaters = new List<IModelUpdater>();

        private readonly ILogger logger;

        public DataModelAssembler(ILogger<DataModelAssembler> logger)
        {
            this.logger = logger;
        }

        public string Name { get; set; }
        public string Id { get; set; }

        public IList<IModelMerger> Mergers { get { return mergers; } }
        public IList<IModelUpdater> Updaters { get { return updaters; } }


        public async Task<DataModel> RunAsync()
        {
            // Create the data model that we'll populate...
            var model = new DataModel
            {
                Id = Id,
                Name = Name
            };

            // Go through all the mergers, and have 'em do the needful.
            foreach (var merger in mergers)
            {
                await merger.RunAsync(model);
            }

            logger.LogDebug("DataModelAssembler, {Name}, entity count = {Num}.", Name, model.Entities.Count());

            // Go through all the updaters, and process 'em
            foreach (var updater in updaters)
            {
                await updater.RunAsync(model);
            }

            // Go through and set the "referenced by" field
            // TODO - turn this into an updater?
            SetReferencedBy(model);

            // Return what we've built
            return model;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0}: {1}", Id, Name);

            using (var childContext = context.Push())
            {
                foreach (var child in mergers)
                {
                    child.Dump(childContext);
                }

                foreach (var child in updaters)
                {
                    child.Dump(childContext);
                }
            }
        }


        private void SetReferencedBy(DataModel model)
        {
            foreach (var entity in model.Entities)
            {
                foreach (var attribute in entity.Attributes)
                {
                    if (attribute.DataType?.Type == PrimitiveType.Ref)
                    {
                        var target = model.FindEntity(attribute.DataType.RefName);

                        if (target != null)
                        {
                            target.ReferencedBy.Add(entity);
                        }
                        else
                        {
                            throw new LoaderException($"Model {model.Id}, {entity.Name}.{attribute.Name} refs {attribute.DataType.RefName}, which does not exist!");
                        }
                    }
                }
            }
        }
    }
}
