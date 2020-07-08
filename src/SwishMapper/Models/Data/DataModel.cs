
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A single data model, typically one of many in a project.
    /// </summary>
    public class DataModel
    {
        private readonly Dictionary<string, DataEntity> entities = new Dictionary<string, DataEntity>();

        public string Name { get; set; }
        public string Id { get; set; }

        public IEnumerable<DataEntity> Entities { get { return entities.Values; } }


        public DataEntity FindOrCreateEntity(string name)
        {
            if (entities.ContainsKey(name))
            {
                return entities[name];
            }

            var entity = new DataEntity
            {
                Name = name
            };

            entities.Add(name, entity);

            return entity;
        }
    }
}
