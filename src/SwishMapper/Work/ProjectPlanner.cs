
using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwishMapper.Models;
using SwishMapper.Models.Project;

namespace SwishMapper.Work
{
    public class ProjectPlanner : IProjectPlanner
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public ProjectPlanner(IServiceProvider serviceProvider,
                              ILogger<ProjectPlanner> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }


        public DataProjectAssembler CreateWork(ProjectDefinition project, AppSettings settings)
        {
            // Grab the temp directory and make sure it exists
            var tempDir = new DirectoryInfo(settings.TempDir);
            if (!tempDir.Exists)
            {
                logger.LogInformation("Creating temp directory: {Path}.", tempDir.FullName);
                tempDir.Create();
            }

            // Create the worker that assembles a data project from its components (models, mappings)
            var root = serviceProvider.GetRequiredService<DataProjectAssembler>();

            // Add a worker to create each data model
            foreach (var model in project.Models)
            {
                root.AddWorker(CreateWorker(model));
            }

            // Add a worker to process each mapping
            // TODO - create work items to process each mapping

            // Dump the dependency tree, for debugging
            using (var writer = new StreamWriter(Path.Combine(tempDir.FullName, "work-plan.txt")))
            using (var context = new PlanDumperContext(writer))
            {
                root.Dump(context);
            }

            // Return the dependency tree, for subsequent execution
            return root;
        }


        private DataModelAssembler CreateWorker(ProjectModel model)
        {
            var worker = serviceProvider.GetRequiredService<DataModelAssembler>();

            worker.Name = model.Name;
            worker.Id = model.Id;

            foreach (var populator in model.Populators)
            {
                switch (populator.Type)
                {
                    case ProjectModelPopulatorType.Csv:
                        worker.Populators.Add(CreateCsvPopulator(populator));
                        break;

                    case ProjectModelPopulatorType.Xsd:
                        worker.Populators.Add(CreateXsdPopulator(populator));
                        break;

                    default:
                        throw new ProjectPlannerException($"Unhandled populator type when planning project: {populator.Type}.");
                }
            }

            return worker;
        }


        private CsvPopulator CreateCsvPopulator(ProjectModelPopulator model)
        {
            var worker = serviceProvider.GetRequiredService<CsvPopulator>();

            worker.Path = model.Path;

            return worker;
        }


        private XsdPopulator CreateXsdPopulator(ProjectModelPopulator model)
        {
            var worker = serviceProvider.GetRequiredService<XsdPopulator>();

            // TODO - verify the path exists and if not, throw a planner exception

            worker.Path = model.Path;
            worker.RootElement = model.RootEntity;

            return worker;
        }
    }
}
