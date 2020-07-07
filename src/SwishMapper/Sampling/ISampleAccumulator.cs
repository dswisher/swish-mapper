
using System.Collections.Generic;

namespace SwishMapper.Sampling
{
    public interface ISampleAccumulator
    {
        IDictionary<string, Sample> Samples { get; }

        void Push(string name, bool isAttribute = false);
        void Pop();
        void SetValue(string val);
    }
}
