
using System.Linq;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;
using SwishMapper.Parsing;

namespace SwishMapper.Cli
{
    public class App
    {
        private readonly IProjectParser parser;
        private readonly ILogger logger;

        public App(IProjectParser parser, ILogger<App> logger)
        {
            this.parser = parser;
            this.logger = logger;
        }


        public void Run(string projectPath)
        {
            // Let 'em know we're starting.
            logger.LogDebug("Run! Project: {Path}", projectPath);

            // Load the project file.
            var project = parser.Parse(projectPath);

            // Load each source and each sink listed in the project file.
            var sources = project.Sources.Select(x => Load(x)).ToList();
            var sinks = project.Sinks.Select(x => Load(x)).ToList();

            // Load the mappings.
            // TODO

            // Generate reports.
            // TODO

            // All done!
            logger.LogDebug("Done.");
        }


        private DataDocument Load(ProjectSource source)
        {
            logger.LogInformation("Loading source {Name}.", source.Path);

            // TODO
            return new DataDocument();
        }


        private DataDocument Load(ProjectSink sink)
        {
            logger.LogInformation("Loading sink {Name}.", sink.Path);

            // TODO
            return new DataDocument();
        }
    }
}
