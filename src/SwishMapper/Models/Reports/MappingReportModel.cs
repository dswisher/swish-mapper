
using System.Collections.Generic;

using SwishMapper.Extensions;
using SwishMapper.Models.Data;

namespace SwishMapper.Models.Reports
{
    public class MappingReportModel
    {
        private readonly Dictionary<string, MappingReportEntity> entities = new Dictionary<string, MappingReportEntity>();

        public string Id { get; set; }
        public string Name { get; set; }


        public IEnumerable<MappingReportEntity> Entities { get { return entities.Values; } }


        public MappingReportEntity FindOrCreateEntity(DataEntity dataEntity)
        {
            return entities.FindOrCreate(dataEntity.Name, () => new MappingReportEntity
            {
                // TODO - freddie - add all the attributes
                Name = dataEntity.Name
            });
        }
    }
}
