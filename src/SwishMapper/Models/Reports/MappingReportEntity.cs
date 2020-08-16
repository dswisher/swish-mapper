
using System.Collections.Generic;

using SwishMapper.Extensions;
using SwishMapper.Models.Data;

namespace SwishMapper.Models.Reports
{
    public class MappingReportEntity
    {
        private readonly Dictionary<string, MappingReportAttribute> attributes = new Dictionary<string, MappingReportAttribute>();

        public string Name { get; set; }
        public string ModelUrl { get; set; }

        public IEnumerable<MappingReportAttribute> Attributes { get { return attributes.Values; } }


        public MappingReportAttribute FindOrCreateAttribute(DataAttribute dataAttribute)
        {
            return attributes.FindOrCreate(dataAttribute.Name, () => new MappingReportAttribute
            {
                Name = dataAttribute.Name,
                DataType = dataAttribute.DataType,
                ModelUrl = $"{dataAttribute.Parent.Parent.Id}.html#{Name}.{dataAttribute.Name}"
            });
        }


        public MappingReportAttribute FindAttribute(string name)
        {
            return attributes.ContainsKey(name) ? attributes[name] : null;
        }
    }
}
