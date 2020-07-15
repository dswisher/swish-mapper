
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class DataAttributeSample
    {
        public string Path { get; set; }
        public string SampleId { get; set; }
        public IEnumerable<string> Top5 { get; set; }
    }
}
