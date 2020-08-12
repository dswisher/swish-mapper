
using System.Collections.Generic;

using SwishMapper.Models.Data;

namespace SwishMapper.Models.Reports
{
    public class MappingReportAttribute
    {
        private readonly List<ExpressiveMapping> maps = new List<ExpressiveMapping>();

        public string Name { get; set; }

        public IList<ExpressiveMapping> Maps { get { return maps; } }

        // TODO - need URL and type for RHS
        // public DataType SinkType { get; set; }
        // public string Url { get; set; }
    }
}
