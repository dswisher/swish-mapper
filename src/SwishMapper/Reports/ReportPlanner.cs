
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SwishMapper.Models.Data;
using SwishMapper.Models.Project;

namespace SwishMapper.Reports
{
    public class ReportPlanner : IReportPlanner
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public ReportPlanner(IServiceProvider serviceProvider,
                             ILogger<ReportPlanner> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }


        public IEnumerable<IReportWorker> CreateWork(DataProject dataProject, ProjectDefinition projectDefinition)
        {
            // Grab the output directory and make sure it exists
            var outDir = new DirectoryInfo(projectDefinition.ReportPath);
            if (!outDir.Exists)
            {
                logger.LogInformation("Creating report output directory: {Path}.", outDir.FullName);
                outDir.Create();
            }

            // Set up the razor engine and pre-compile the templates.
            logger.LogInformation("Setting up Razor and compiling templates.");

            var watch = Stopwatch.StartNew();
            var razorConfig = new TemplateServiceConfiguration
            {
                TemplateManager = new RazorTemplateManager()
            };

            var razorService = RazorEngineService.Create(razorConfig);

            razorService.Compile("model-report", typeof(DataModel));

            Engine.Razor = razorService;

            logger.LogDebug("...razor setup complete, elapsed: {Time}.", watch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));

            // Copy static (embedded) files to the output directory
            yield return serviceProvider.CopyEmbedded("style.css", outDir.FullName);

            // Create a model report for each model
            foreach (var model in dataProject.Models)
            {
                var path = Path.Combine(outDir.FullName, $"{model.Id}.html");

                // TODO - resolve ModelReport from service provider and set props!
                yield return new ModelReport(model, path);
            }

            // TODO - create an index page
        }
    }
}
