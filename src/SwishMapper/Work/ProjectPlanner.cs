
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
            foreach (var map in project.Maps)
            {
                root.AddWorker(CreateWorker(map));
            }

            // Dump the dependency tree, for debugging
            using (var writer = new StreamWriter(Path.Combine(tempDir.FullName, "work-plan.txt")))
            using (var context = new PlanDumperContext(writer))
            {
                root.Dump(context);
            }

            // Return the dependency tree, for subsequent execution
            return root;
        }


        private MapLoader CreateWorker(ProjectMap map)
        {
            var worker = serviceProvider.GetRequiredService<MapLoader>();

            worker.FromModelId = map.FromModelId;
            worker.ToModelId = map.ToModelId;
            worker.Path = map.Path;

            return worker;
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
                        worker.Mergers.Add(CreateCsvLoader(populator, model));
                        break;

                    case ProjectModelPopulatorType.Xsd:
                        worker.Mergers.Add(CreateXsdLoader(populator, model));
                        break;

                    default:
                        throw new ProjectPlannerException($"Unhandled populator type when planning project: {populator.Type}.");
                }
            }

            return worker;
        }


        private IModelMerger CreateCsvLoader(ProjectModelPopulator modelPopulator, ProjectModel projectModel)
        {
            // Create the loader
            var loader = serviceProvider.GetRequiredService<CsvLoader>();

            // TODO - verify the path exists, and if not, throw an exception

            loader.Path = modelPopulator.Path;
            loader.ModelId = projectModel.Id;
            loader.ModelName = projectModel.Name;

            // Wrap the loader in a cleaner
            var cleaner = serviceProvider.GetRequiredService<ModelCleaner>();

            cleaner.Input = loader;

            // Wrap the cleaner in a merger
            var merger = serviceProvider.GetRequiredService<ModelMerger>();

            merger.Input = cleaner;

            return merger;
        }


        private IModelMerger CreateXsdLoader(ProjectModelPopulator modelPopulator, ProjectModel projectModel)
        {
            // Create the loader
            var loader = serviceProvider.GetRequiredService<XsdLoader>();

            // TODO - verify the path exists, and if not, throw an exception

            loader.Path = modelPopulator.Path;
            loader.RootElement = modelPopulator.RootEntity;
            loader.ModelId = projectModel.Id;
            loader.ModelName = projectModel.Name;

            // Wrap the loader in a cleaner
            var cleaner = serviceProvider.GetRequiredService<ModelCleaner>();

            cleaner.Input = loader;

            // Wrap the cleaner in a merger
            var merger = serviceProvider.GetRequiredService<ModelMerger>();

            merger.Input = cleaner;

            return merger;
        }
    }
}
