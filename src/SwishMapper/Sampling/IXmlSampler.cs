
using System.Threading.Tasks;

namespace SwishMapper.Sampling
{
    public interface IXmlSampler
    {
        Task SampleAsync(SampleStream stream, ISampleAccumulator accumulator);
    }
}
