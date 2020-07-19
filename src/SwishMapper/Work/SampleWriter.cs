
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;
using SwishMapper.Models.Project;
using SwishMapper.Sampling;

namespace SwishMapper.Work
{
    public class SampleWriter
    {
        private readonly ISampleStreamFinder finder;
        private readonly ISampleAccumulator accumulator;
        private readonly IXmlSampler xmlSampler;
        private readonly ILogger logger;

        public SampleWriter(ISampleStreamFinder finder,
                            ISampleAccumulator accumulator,
                            IXmlSampler xmlSampler,
                            ILogger<SampleWriter> logger)
        {
            this.finder = finder;
            this.accumulator = accumulator;
            this.xmlSampler = xmlSampler;
            this.logger = logger;
        }


        public string OutputPath { get; set; }
        public IEnumerable<SampleInputFile> InputFiles { get; set; }
        public string ZipMask { get; set; }


        public async Task<SampleJson> RunAsync()
        {
            // Build the finder options
            var finderOptions = new SampleStreamFinderOptions
            {
                InputFiles = InputFiles,
                ZipMask = ZipMask
            };

            // Determine if the output file exists and is already up to date.
            if (OutputIsAlreadyCurrent())
            {
                logger.LogInformation("Sample output {Name} is up to date, skipping regeneration.", Path.GetFileName(OutputPath));

                using (var stream = new FileStream(OutputPath, FileMode.Open, FileAccess.Read))
                {
                    var content = await JsonSerializer.DeserializeAsync<SampleJson>(stream);

                    logger.LogDebug("Loaded samples from {Name}, found {Num1} filenames and {Num2} data points.",
                            OutputPath, content.Filenames.Count, content.DataPoints.Count);

                    return content;
                }
            }

            // Go through all the inputs, and process 'em
            foreach (var sample in finder.Find(finderOptions))
            {
                using (sample)
                {
                    // TODO - parallelize this? In theory, it's all async...
                    // TODO - based on the file type, use the proper type of sampler; for now, it's XML or go home.
                    logger.LogDebug("Sample file: {Name}", sample.Filename);

                    accumulator.AddFile(Path.GetFileName(sample.Filename));

                    await xmlSampler.SampleAsync(sample, accumulator);
                }
            }

            // Build the JSON representation of the sample data
            var json = accumulator.AsJson();

            // Write the sample info to the output file
            logger.LogInformation("Writing sample details to {Name}.", OutputPath);

            using (var stream = new FileStream(OutputPath, FileMode.Create, FileAccess.Write))
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                await JsonSerializer.SerializeAsync(stream, json, jsonOptions);
            }

            return json;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "TBD");
        }


        private bool OutputIsAlreadyCurrent()
        {
            // If the output file does not exist, it can't really be considered up-to-date.
            var outputInfo = new FileInfo(OutputPath);
            if (!outputInfo.Exists)
            {
                logger.LogDebug("-> sample output does not exist!");
                return false;
            }

            // If any of the input files are newer than the output file, the output file is out-of-date.
            foreach (var file in InputFiles)
            {
                if (file.LastWriteUtc > outputInfo.LastWriteTimeUtc)
                {
                    logger.LogDebug("-> sample input file {Name} ({Date1}) is newer than output file ({Date2}).",
                            Path.GetFileName(file.Path), file.LastWriteUtc, outputInfo.LastWriteTimeUtc);
                    return false;
                }
            }

            // Looks like everything is up to date.
            return true;
        }
    }
}
