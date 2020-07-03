
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
        private readonly IMappingParser mappingParser;
        private readonly ILogger logger;

        public App(IProjectParser projectParser,
                   IXsdParser xsdParser,
                   IMappingParser mappingParser,
                   ILogger<App> logger)
        {
            this.projectParser = projectParser;
            this.xsdParser = xsdParser;
            this.mappingParser = mappingParser;
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
            foreach (var pm in project.Mappings)
            {
                var map = await Load(pm, sources, sinks);

                // TODO - keep the map around, or hook it onto the sink?
            }

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

            // Copy the static files to the OUTPUT directory
            var content = new DirectoryInfo("Content");
            foreach (var file in content.GetFiles())
            {
                // TODO - only copy if source is newer
                var target = Path.Combine(output.FullName, file.Name);
                file.CopyTo(target, true);
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


        private async Task<object> Load(ProjectMapping projectMapping, IEnumerable<DataDocument> sources, IEnumerable<DataDocument> sinks)
        {
            logger.LogInformation("Loading mapping {Name} from {Path}.", projectMapping.ProjectPath, projectMapping.FullPath);

            var map = await mappingParser.ParseAsync(projectMapping.FullPath, sources, sinks);

            // TODO
            await Task.CompletedTask;
            return null;
        }
    }
}
