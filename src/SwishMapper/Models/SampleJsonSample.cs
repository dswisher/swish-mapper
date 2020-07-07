
namespace SwishMapper.Models
{
    /// <summary>
    /// A single sample and associated count.
    /// </summary>
    public class SampleJsonSample
    {
        /// <summary>
        /// The value of the sample.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The number of occurrences of this sample.
        /// </summary>
        public int Count { get; set; }
    }
}
