
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// A single item in the sample JSON (one "xpath").
    /// </summary>
    public class SampleJsonItem
    {
        /// <summary>
        /// The name for the sample. Essentially the "xpath".
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The total number of values seen for this item.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The number of distinct values seen for this item.
        /// </summary>
        public int Uniques { get; set; }

        /// <summary>
        /// The number of samples in the samples list
        /// </summary>
        public int NumSamples { get; set; }

        /// <summary>
        /// Sample values for this data point.
        /// </summary>
        public List<SampleJsonSample> Samples { get; set; }
    }
}
