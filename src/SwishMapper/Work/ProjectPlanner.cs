
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
                root.AddWorker(CreateWorker(model, settings));
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


        private DataModelAssembler CreateWorker(ProjectModel model, AppSettings settings)
        {
            var worker = serviceProvider.GetRequiredService<DataModelAssembler>();

            worker.Name = model.Name;
            worker.Id = model.Id;

            foreach (var populator in model.Populators)
            {
                switch (populator.Type)
                {
                    case ProjectModelPopulatorType.Csv:
                        worker.Mergers.Add(CreateCsvLoader((CsvProjectModelPopulator)populator, model, settings));
                        break;

                    case ProjectModelPopulatorType.Xsd:
                        worker.Mergers.Add(CreateXsdLoader((XsdProjectModelPopulator)populator, model, settings));
                        break;

                    case ProjectModelPopulatorType.Sample:
                        worker.Updaters.Add(CreateSampleUpdater((SampleProjectModelPopulator)populator, model, settings));
                        break;

                    default:
                        throw new ProjectPlannerException($"Unhandled populator type when planning project: {populator.Type}.");
                }
            }

            return worker;
        }


        private IModelUpdater CreateSampleUpdater(SampleProjectModelPopulator modelPopulator, ProjectModel projectModel, AppSettings settings)
        {
            // Going through lots of files to gather up the samples takes quite some time. To amortize
            // the impact, we do the sampling and write a summary to the scratch area. The normal pipeline
            // reads this summary and loads it into the model. The process begins by creating a sample writer...
            var writer = serviceProvider.GetRequiredService<SampleWriter>();

            writer.OutputPath = Path.Combine(settings.TempDir, $"{projectModel.Id}-{modelPopulator.Id}.json");
            writer.InputFiles = modelPopulator.Files;
            writer.ZipMask = modelPopulator.ZipMask;

            // Create the updater, which reads the samples and updates the model
            var updater = serviceProvider.GetRequiredService<IModelSampleUpdater>();

            updater.Writer = writer;
            updater.SampleId = modelPopulator.Id;

            return updater;
        }


        private IModelMerger CreateCsvLoader(CsvProjectModelPopulator modelPopulator, ProjectModel projectModel, AppSettings settings)
        {
            // Create the normalizer. This reads the CSV file itself and transforms the rows into a
            // fixed format that will be consumed by the loader.
            var normalizer = serviceProvider.GetRequiredService<ICsvNormalizer>();

            // TODO - verify the path exists, and if not, throw an exception

            normalizer.Path = modelPopulator.Path;

            // Create the CSV-to-XSD translator
            var csvToXsd = serviceProvider.GetRequiredService<ICsvToXsdTranslator>();

            csvToXsd.Input = normalizer;

            // TODO - make debugging an optional flag, either on the command-line or perhaps in the project file
            csvToXsd.DebugDumpPath = Path.Combine(settings.TempDir, $"{projectModel.Id}-csvToXsd.json");

            // Create the XSD-to-model translator
            var xsdToModel = XsdToModel(csvToXsd, modelPopulator.Path, projectModel);

            xsdToModel.ShortName = "csv";

            // Clean things up a bit
            var cleaner = Clean(xsdToModel);

            // Wrap it all up in a merger
            return Merge(cleaner);
        }


        private IModelMerger CreateXsdLoader(XsdProjectModelPopulator modelPopulator, ProjectModel projectModel, AppSettings settings)
        {
            // Create the loader
            var loader = serviceProvider.GetRequiredService<IXsdLoader>();

            // TODO - verify the path exists, and if not, throw an exception

            loader.Path = modelPopulator.Path;

            // TODO - make debugging an optional flag, either on the command-line or perhaps in the project file
            loader.DebugDumpPath = Path.Combine(settings.TempDir, $"{projectModel.Id}-xsdLoader.json");

            // Create the XSD-to-model translator
            var xsdToModel = XsdToModel(loader, modelPopulator.Path, projectModel);

            xsdToModel.ShortName = "xsd";

            // Clean things up a bit
            var cleaner = Clean(xsdToModel);

            // Wrap it all up in a merger
            return Merge(cleaner);
        }


        private IXsdToModelTranslator XsdToModel(IWorker<XsdDocument> input, string path, ProjectModel projectModel)
        {
            var xsdToModel = serviceProvider.GetRequiredService<IXsdToModelTranslator>();

            xsdToModel.Input = input;
            xsdToModel.Path = path;
            xsdToModel.ModelId = projectModel.Id;
            xsdToModel.ModelName = projectModel.Name;

            return xsdToModel;
        }


        private IModelProducer Clean(IModelProducer input)
        {
            var cleaner = serviceProvider.GetRequiredService<IEmptyEntityCleaner>();

            cleaner.Input = input;

            return cleaner;
        }


        private IModelMerger Merge(IModelProducer input)
        {
            var merger = serviceProvider.GetRequiredService<IModelMerger>();

            merger.Input = input;

            return merger;
        }
    }
}
