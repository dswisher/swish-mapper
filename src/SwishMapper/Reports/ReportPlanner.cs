
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SwishMapper.Models;
using SwishMapper.Models.Data;
using SwishMapper.Models.Reports;

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


        public IEnumerable<IReportWorker> CreateWork(DataProject dataProject, AppSettings settings)
        {
            // Grab the output directory and make sure it exists
            var outDir = new DirectoryInfo(settings.ReportDir);
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

            razorService.Compile("mapping-report", typeof(MappingReportModel));
            razorService.Compile("model-report", typeof(DataModel));
            razorService.Compile("index-page", typeof(IndexModel));

            Engine.Razor = razorService;

            logger.LogDebug("...razor setup complete, elapsed: {Time}.", watch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));

            // Create the index data model, so we can add to it as we're planning out other stuff
            var indexModel = new IndexModel();

            // Copy static (embedded) files to the output directory
            yield return serviceProvider.CopyEmbedded("style.css", outDir.FullName);

            // Create a model report for each model
            var section = indexModel.CreateSection("Models");

            foreach (var model in dataProject.Models)
            {
                var filename = $"{model.Id}.html";
                var path = Path.Combine(outDir.FullName, filename);

                section.CreateEntry(model.Name, filename);

                yield return serviceProvider.ModelReport(model, path);
            }

            // Create a mapping report for each mapping
            section = indexModel.CreateSection("Mappings");

            foreach (var map in dataProject.Maps)
            {
                // TODO - what if there are two maps between the same models? Need more foolproof naming.
                var filename = $"{map.SourceModel.Id}-{map.SinkModel.Id}.html";
                var path = Path.Combine(outDir.FullName, filename);

                section.CreateEntry($"{map.SourceModel.Name} -> {map.SinkModel.Name}", filename);

                yield return serviceProvider.MappingReport(map, path);
            }

            // Create the index page
            yield return serviceProvider.IndexPage(indexModel, Path.Combine(outDir.FullName, "index.html"));
        }
    }
}
