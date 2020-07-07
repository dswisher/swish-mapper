
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// The model for a file full of sample data.
    /// </summary>
    public class SampleJson
    {
        private readonly List<SampleJsonItem> dataPoints = new List<SampleJsonItem>();
        private readonly List<string> filenames = new List<string>();

        public int NumFiles { get; set; }
        public int NumDataPoints { get; set; }

        public IList<string> Filenames { get { return filenames; } }
        public IList<SampleJsonItem> DataPoints { get { return dataPoints; } }
    }
}
