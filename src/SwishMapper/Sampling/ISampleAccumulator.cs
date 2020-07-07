
using System.Collections.Generic;

using SwishMapper.Models;

namespace SwishMapper.Sampling
{
    public interface ISampleAccumulator
    {
        IDictionary<string, Sample> Samples { get; }
        IList<string> Filenames { get; }

        void AddFile(string filename);

        void Push(string name, bool isAttribute = false);
        void Pop();
        void SetValue(string val);

        SampleJson AsJson();
    }
}
