
using System.Collections.Generic;

namespace SwishMapper.Sampling
{
    public class Sample
    {
        private readonly Dictionary<string, int> histogram = new Dictionary<string, int>();


        public int TotalSeen { get; private set; }
        public IDictionary<string, int> Histogram { get { return histogram; } }


        public void Add(string val)
        {
            if (histogram.ContainsKey(val))
            {
                histogram[val] += 1;
            }
            else
            {
                histogram.Add(val, 1);
            }

            TotalSeen += 1;
        }
    }
}
