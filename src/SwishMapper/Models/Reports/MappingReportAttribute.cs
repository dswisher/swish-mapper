
using System.Collections.Generic;

using SwishMapper.Models.Data;

namespace SwishMapper.Models.Reports
{
    public class MappingReportAttribute
    {
        private readonly List<DataAttributeMap> maps = new List<DataAttributeMap>();

        public string Name { get; set; }

        public IList<DataAttributeMap> Maps { get { return maps; } }
    }
}
