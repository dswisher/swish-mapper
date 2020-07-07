
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Sampling;

namespace SwishMapper.Sampler
{
    public class App
    {
        private readonly ISampleStreamFinder finder;
        private readonly ISampleAccumulator accumulator;
        private readonly IXmlSampler xmlSampler;
        private readonly ILogger logger;


        public App(ISampleStreamFinder finder,
                   ISampleAccumulator accumulator,
                   IXmlSampler xmlSampler,
                   ILogger<App> logger)
        {
            this.finder = finder;
            this.accumulator = accumulator;
            this.xmlSampler = xmlSampler;
            this.logger = logger;
        }


        public async Task RunAsync(Options options)
        {
            var watch = Stopwatch.StartNew();

            // Build the finder options
            var finderOptions = new SampleStreamFinderOptions
            {
                InputFiles = options.InputFiles,
                ZipMask = options.ZipMask
            };

            // Go through all the inputs, and process 'em
            foreach (var sample in finder.Find(finderOptions))
            {
                using (sample)
                {
                    // TODO - parallelize this? In theory, it's all async...
                    // TODO - based on the file type, use the proper type of sampler; for now, it's XML or go home.
                    logger.LogInformation("Sample file: {Name}", sample.Filename);
                    await xmlSampler.SampleAsync(sample, accumulator);
                }
            }

            // TODO - "extract" the sample data from the accumulator and return it

            logger.LogInformation($"Elapsed time: {watch.Elapsed:hh\\:mm\\:ss\\.fff}");
        }
    }
}
