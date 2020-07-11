
using System.Collections.Generic;
using System.Linq;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A single data model, typically one of many in a project.
    /// </summary>
    public class DataModel
    {
        private readonly Dictionary<string, DataEntity> entities = new Dictionary<string, DataEntity>();
        private readonly List<DataModelSource> sources = new List<DataModelSource>();


        public string Name { get; set; }
        public string Id { get; set; }

        public IEnumerable<DataEntity> Entities { get { return entities.Values; } }
        public IList<DataModelSource> Sources { get { return sources; } }


        public DataEntity FindEntity(string name)
        {
            return entities.ContainsKey(name) ? entities[name] : null;
        }


        public DataEntity FindOrCreateEntity(string name, DataModelSource source)
        {
            // Locate an existing entity or create a new one.
            DataEntity entity;

            if (entities.ContainsKey(name))
            {
                entity = entities[name];
            }
            else
            {
                entity = new DataEntity(this, name);

                entities.Add(name, entity);
            }

            // Add the source
            if (!entity.Sources.Any(x => x.ShortName == source.ShortName))
            {
                entity.Sources.Add(source);
            }

            // Return what we've got!
            return entity;
        }
    }
}
