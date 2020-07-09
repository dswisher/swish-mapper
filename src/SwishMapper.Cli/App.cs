
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Parsing.Project;
using SwishMapper.Reports;
using SwishMapper.Work;

namespace SwishMapper.Cli
{
    public class App
    {
        private readonly IProjectParser projectParser;
        private readonly IProjectPlanner projectPlanner;
        private readonly IReportPlanner reportPlanner;
        private readonly ILogger logger;

        public App(IProjectParser projectParser,
                   IProjectPlanner projectPlanner,
                   IReportPlanner reportPlanner,
                   ILogger<App> logger)
        {
            this.projectParser = projectParser;
            this.projectPlanner = projectPlanner;
            this.reportPlanner = reportPlanner;
            this.logger = logger;
        }


        public async Task RunAsync(string projectPath)
        {
            // Let 'em know we're starting.
            logger.LogDebug("Run! Project: {Path}", projectPath);

            var timer = Stopwatch.StartNew();

            // Parse the project file to get a model of the project
            var projectModel = await projectParser.ParseAsync(projectPath);

            // Build a dependency graph from the project model
            var work = projectPlanner.CreateWork(projectModel);

            // Execute the dependency graph to get a data project
            var dataProject = await work.RunAsync();

            // Create the list of report tasks from the project model
            var reports = reportPlanner.CreateWork(dataProject, projectModel);

            // Generate all the desired reports
            await Task.WhenAll(reports.Select(x => x.RunAsync()));

            // All done!
            logger.LogDebug("Done.");
            logger.LogDebug("Elapsed time: {Time}", timer.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
        }
    }
}
