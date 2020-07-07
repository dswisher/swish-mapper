
using System.Collections.Generic;
using System.IO;
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
            foreach (var path in options.InputFiles)
            {
                logger.LogDebug("...opening {Name}...", path);

                // TODO - handle zip files!

                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                yield return new SampleStream(path, stream, x => logger.LogDebug("...closing {Name}...", x.Filename));
            }
        }
    }
}
