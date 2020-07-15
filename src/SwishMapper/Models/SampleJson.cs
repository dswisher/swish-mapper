
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// The model for a file full of sample data.
    /// </summary>
    public class SampleJson
    {
        public int NumFiles { get; set; }
        public int NumDataPoints { get; set; }

        public List<string> Filenames { get; set; } = new List<string>();
        public List<SampleJsonItem> DataPoints { get; set; } = new List<SampleJsonItem>();
    }
}
