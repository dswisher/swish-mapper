
using System;
using System.Collections.Generic;
using System.Linq;

using SwishMapper.Models;

namespace SwishMapper.Sampling
{
    public class SampleAccumulator : ISampleAccumulator
    {
        private readonly object mutex = new object();
        private readonly Stack<string> stack = new Stack<string>();
        private readonly Dictionary<string, Sample> samples = new Dictionary<string, Sample>();
        private readonly List<string> filenames = new List<string>();


        public IDictionary<string, Sample> Samples { get { return samples; } }
        public IList<string> Filenames { get { return filenames; } }


        public void AddFile(string filename)
        {
            filenames.Add(filename);
        }


        public void Push(string name, bool isAttribute = false)
        {
            lock (mutex)
            {
                // TODO - implement Push attribute
                stack.Push(name);
            }
        }


        public void Pop()
        {
            lock (mutex)
            {
                stack.Pop();
            }
        }


        public void SetValue(string val)
        {
            lock (mutex)
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


        public SampleJson AsJson()
        {
            lock (mutex)
            {
                var json = new SampleJson
                {
                    NumFiles = filenames.Count,
                    NumDataPoints = samples.Count
                };

                foreach (var filename in filenames)
                {
                    json.Filenames.Add(filename);
                }

                foreach (var pair in samples.OrderBy(x => x.Key))
                {
                    // Determine how many samples we want
                    var total = pair.Value.TotalSeen;
                    var uniques = pair.Value.Histogram.Count;

                    var desiredSamples = 5;
                    if ((uniques < 50) || (uniques < total * 0.01))
                    {
                        desiredSamples = uniques;
                    }

                    // Create the item and add it to the result
                    var item = new SampleJsonItem
                    {
                        Path = pair.Key,
                        Total = total,
                        Uniques = uniques,
                        NumSamples = Math.Min(desiredSamples, pair.Value.Histogram.Count),
                        Samples = pair.Value.Histogram
                            .OrderByDescending(x => x.Value)
                            .Take(desiredSamples)
                            .Select(x => new SampleJsonSample { Value = x.Key, Count = x.Value })
                            .ToList()
                    };

                    json.DataPoints.Add(item);
                }

                return json;
            }
        }
    }
}
