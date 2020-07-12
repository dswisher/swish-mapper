
using System.Collections.Generic;

namespace SwishMapper.Models.Reports
{
    public class MappingReportEntity
    {
        private readonly Dictionary<string, MappingReportAttribute> attributes = new Dictionary<string, MappingReportAttribute>();

        public string Name { get; set; }

        public IEnumerable<MappingReportAttribute> Attributes { get { return attributes.Values; } }


        public MappingReportAttribute FindOrCreateAttribute(string name)
        {
            if (attributes.ContainsKey(name))
            {
                return attributes[name];
            }

            var attribute = new MappingReportAttribute
            {
                Name = name
            };

            attributes.Add(name, attribute);

            return attribute;
        }
    }
}
