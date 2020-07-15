
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace SwishMapper.Sampling
{
    public class SampleStreamFinder : ISampleStreamFinder
    {
        private readonly ILogger logger;

        public SampleStreamFinder(ILogger<SampleStreamFinder> logger)
        {
            this.logger = logger;
        }


        public IEnumerable<SampleStream> Find(SampleStreamFinderOptions options)
        {
            Regex zipMask = null;
            if (!string.IsNullOrEmpty(options.ZipMask))
            {
                zipMask = new Regex(options.ZipMask, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            foreach (var file in options.InputFiles)
            {
                var path = file.Path;

                if (path.EndsWith(".zip"))
                {
                    using (var zip = ZipFile.OpenRead(path))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            if (!Path.GetExtension(entry.Name).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            if ((zipMask != null) && !zipMask.IsMatch(entry.Name))
                            {
                                continue;
                            }

                            yield return new SampleStream(entry.Name, entry.Open());
                        }
                    }
                }
                else
                {
                    var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                    yield return new SampleStream(path, stream);
                }
            }
        }
    }
}
