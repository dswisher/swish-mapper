
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Formatters;
using SwishMapper.Models;
using SwishMapper.Parsing;
using SwishMapper.Reports;

namespace SwishMapper.Cli
{
    public class App
    {
        private readonly IProjectParser projectParser;
        private readonly IXsdParser xsdParser;
        private readonly ILogger logger;

        public App(IProjectParser projectParser,
                   IXsdParser xsdParser,
                   ILogger<App> logger)
        {
            this.projectParser = projectParser;
            this.xsdParser = xsdParser;
            this.logger = logger;
        }


        public async Task RunAsync(string projectPath)
        {
            // TODO - split this out so it can be unit tested

            // Let 'em know we're starting.
            logger.LogDebug("Run! Project: {Path}", projectPath);

            // Load the project file.
            var project = projectParser.Parse(projectPath);

            // Load each source and each sink listed in the project file.
            // TODO - parallelize this!
            var sources = new List<DataDocument>();
            var sinks = new List<DataDocument>();

            foreach (var ps in project.Sources)
            {
                sources.Add(await Load(ps));
            }

            foreach (var ps in project.Sinks)
            {
                sinks.Add(await Load(ps));
            }

            // Load the mappings.
            // TODO

            // Generate reports.
            var output = new DirectoryInfo("OUTPUT");
            if (!output.Exists)
            {
                logger.LogInformation("Creating output directory {Dir}.", output.FullName);
                output.Create();
            }

            foreach (var doc in sources.Concat(sinks))
            {
                var report = new DocumentReport(doc);
                var reportPath = Path.Combine(output.FullName, $"doc-{doc.Name}.html");

                logger.LogInformation("Writing doc report for {Name} to {Path}", doc.Name, reportPath);

                using (var writer = new StreamWriter(reportPath))
                {
                    var formatter = new HtmlFormatter(writer);

                    formatter.Write(report);
                }
            }

            // All done!
            logger.LogDebug("Done.");
        }


        private async Task<DataDocument> Load(ProjectDocument projectDoc)
        {
            logger.LogInformation("Loading {Kind} {Name}.", projectDoc is ProjectSink ? "sink" : "source", projectDoc.Name);

            // TODO - what if the source is not an XSD?
            // TODO - need the root namespace!
            var doc = await xsdParser.ParseAsync(projectDoc.FullPath, projectDoc.Name, projectDoc.RootElementName, string.Empty);

            return doc;
        }
    }
}
