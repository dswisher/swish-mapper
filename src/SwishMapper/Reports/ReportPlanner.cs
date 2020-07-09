
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
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


        public IEnumerable<IReport> CreateWork(DataProject dataProject, ProjectDefinition projectDefinition)
        {
            // Grab the output directory and make sure it exists
            var outDir = new DirectoryInfo(projectDefinition.ReportPath);
            if (!outDir.Exists)
            {
                // TODO - log it
                outDir.Create();
            }

            // Create a model report for each model
            foreach (var model in dataProject.Models)
            {
                var path = Path.Combine(outDir.FullName, $"{model.Id}.html");

                yield return new ModelReport(model, path);
            }

            // TODO - create an index page
        }
    }
}
