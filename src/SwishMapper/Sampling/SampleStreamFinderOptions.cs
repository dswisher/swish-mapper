
using System.Collections.Generic;

namespace SwishMapper.Sampling
{
    public class SampleStreamFinderOptions
    {
        public IEnumerable<string> InputFiles { get; set; }
        public string ZipMask { get; set; }
    }
}
