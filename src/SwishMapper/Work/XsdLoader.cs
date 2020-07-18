
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;
using SwishMapper.Parsing.Xsd;

namespace SwishMapper.Work
{
    public class XsdLoader : IXsdLoader
    {
        private readonly IXsdParser parser;
        private readonly ILogger logger;

        public XsdLoader(IXsdParser parser,
                         ILogger<XsdLoader> logger)
        {
            this.parser = parser;
            this.logger = logger;
        }


        public string Path { get; set; }
        public string DebugDumpPath { get; set; }


        public async Task<XsdDocument> RunAsync()
        {
            // Parse the XML schema document
            // TODO - remove RootElement as a parser parameter - just return ALL elements
            var doc = await parser.ParseAsync(Path);

            // If a debug path was specified, write the doc there for post-mortem analysis...
            if (!string.IsNullOrEmpty(DebugDumpPath))
            {
                using (var stream = new FileStream(DebugDumpPath, FileMode.Create, FileAccess.Write))
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    try
                    {
                        await JsonSerializer.SerializeAsync(stream, doc, jsonOptions);
                    }
                    catch (JsonException ex)
                    {
                        logger.LogWarning("JSON Exception saving XSD debug dump: {Message}", ex.Message);
                    }
                }
            }

            // Return what we've built
            return doc;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0}", Path);
        }
    }
}
