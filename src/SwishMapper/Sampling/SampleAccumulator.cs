
using System.Collections.Generic;
using System.Linq;

namespace SwishMapper.Sampling
{
    public class SampleAccumulator : ISampleAccumulator
    {
        private readonly Stack<string> stack = new Stack<string>();
        private readonly Dictionary<string, Sample> samples = new Dictionary<string, Sample>();


        public IDictionary<string, Sample> Samples { get { return samples; } }


        public void Push(string name, bool isAttribute = false)
        {
            // TODO - implement Push attribute
            stack.Push(name);
        }


        public void Pop()
        {
            stack.Pop();
        }


        public void SetValue(string val)
        {
            var key = string.Join("/", stack.Reverse());

            Sample sample;
            if (samples.ContainsKey(key))
            {
                sample = samples[key];
            }
            else
            {
                sample = new Sample();
                samples.Add(key, sample);
            }

            sample.Add(val);
        }
    }
}
