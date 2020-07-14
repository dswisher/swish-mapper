
using System.Collections.Generic;

using SwishMapper.Models.Project;

namespace SwishMapper.Sampling
{
    public class SampleStreamFinderOptions
    {
        public IEnumerable<SampleInputFile> InputFiles { get; set; }
        public string ZipMask { get; set; }
    }
}
