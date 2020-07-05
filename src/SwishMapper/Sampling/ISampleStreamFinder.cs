
using System.Collections.Generic;

namespace SwishMapper.Sampling
{
    public interface ISampleStreamFinder
    {
        IEnumerable<SampleStream> Find(SampleStreamFinderOptions options);
    }
}
