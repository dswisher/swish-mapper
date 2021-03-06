
using System.Collections.Generic;

using SwishMapper.Models.Data;

namespace SwishMapper.Models.Reports
{
    public class MappingReportAttribute
    {
        private readonly List<ExpressiveMapping> maps = new List<ExpressiveMapping>();
        private readonly List<MappingReportNote> notes = new List<MappingReportNote>();
        private readonly List<MappingReportExample> examples = new List<MappingReportExample>();

        public string Name { get; set; }
        public DataType DataType { get; set; }
        public string ModelUrl { get; set; }

        public IList<ExpressiveMapping> Maps { get { return maps; } }
        public IList<MappingReportNote> Notes { get { return notes; } }
        public IList<MappingReportExample> Examples { get { return examples; } }
    }
}
