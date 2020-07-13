
using System.Collections.Generic;

namespace SwishMapper.Models.Reports
{
    public class MappingReportModel
    {
        private readonly Dictionary<string, MappingReportEntity> entities = new Dictionary<string, MappingReportEntity>();

        public string Name { get; set; }


        public IEnumerable<MappingReportEntity> Entities { get { return entities.Values; } }


        public MappingReportEntity FindOrCreateEntity(string name)
        {
            if (entities.ContainsKey(name))
            {
                return entities[name];
            }

            var entity = new MappingReportEntity
            {
                Name = name
            };

            entities.Add(name, entity);

            return entity;
        }
    }
}
