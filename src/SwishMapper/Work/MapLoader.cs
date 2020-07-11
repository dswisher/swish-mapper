
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CsvHelper;
using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class MapLoader
    {
        private readonly ILogger logger;

        public MapLoader(ILogger<MapLoader> logger)
        {
            this.logger = logger;
        }


        public string FromModelId { get; set; }
        public string ToModelId { get; set; }
        public string Path { get; set; }


        public async Task RunAsync(DataProject project)
        {
            logger.LogDebug($"Loading mapping file {Path}.");

            // Find and validate the source and sink
            var sourceModel = project.Models.FirstOrDefault(x => x.Id == FromModelId);
            var sinkModel = project.Models.FirstOrDefault(x => x.Id == ToModelId);

            if (sourceModel == null)
            {
                // TODO - keep track of project-file line-number, so we can report it here
                throw new LoaderException($"Could not locate 'from' model with ID '{FromModelId}'.");
            }

            if (sinkModel == null)
            {
                // TODO - keep track of project-file line-number, so we can report it here
                throw new LoaderException($"Could not locate 'to' model with ID '{ToModelId}'.");
            }

            // Create the map object and stuff it into the project.
            var dataMap = new DataMapping
            {
                SourceModel = sourceModel,
                SinkModel = sinkModel
            };

            project.Maps.Add(dataMap);

            // Load and process the mappings
            using (var reader = new StreamReader(Path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var line = 0;

                // Skip the header row
                await csv.ReadAsync();
                line += 1;

                // If we try to read a column that does not exist (comments), don't throw a fit.
                csv.Configuration.MissingFieldFound = null;

                // Process all the lines
                while (await csv.ReadAsync())
                {
                    line += 1;

                    // Pull out the values
                    var sourceXPath = csv.GetField(0);
                    var sinkXPath = csv.GetField(1);
                    var comments = csv.GetField(2);

                    // Validate
                    if (string.IsNullOrEmpty(sourceXPath))
                    {
                        throw new LoaderException($"Line {line} in {Path} is invalid: source XPath is empty/null.");
                    }

                    if (string.IsNullOrEmpty(sinkXPath))
                    {
                        throw new LoaderException($"Line {line} in {Path} is invalid: sink XPath is empty/null.");
                    }

                    // Find the attributes
                    var sourceAtt = FindAttribute(line, sourceModel, sourceXPath);
                    var sinkAtt = FindAttribute(line, sinkModel, sinkXPath);

                    var map = new DataAttributeMap
                    {
                        FromAttribute = sourceAtt,
                        FromXPath = sourceXPath,
                        ToAttribute = sinkAtt,
                        ToXPath = sinkXPath,
                        Comments = comments
                    };

                    sourceAtt.OutgoingMaps.Add(map);
                    sinkAtt.IncomingMaps.Add(map);

                    dataMap.Maps.Add(map);
                }
            }
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0} -> {1}", FromModelId, ToModelId);
        }


        private DataAttribute FindAttribute(int line, DataModel model, string xpath)
        {
            // Grab the last two parts of the xpath.
            var bits = xpath.Split('/');

            if (bits.Length < 2)
            {
                throw new LoaderException($"Line {line} in {Path} has an invalid xpath (too few segments): '{xpath}'.");
            }

            var entityName = bits[bits.Length - 2];
            var attributeName = bits[bits.Length - 1];

            var entity = model.FindEntity(entityName);

            if (entity == null)
            {
                throw new LoaderException($"Could not find entity '{entityName}' for xpath '{xpath}' in model '{model.Id}'.");
            }

            var attribute = entity.FindAttribute(attributeName);

            if (attribute == null)
            {
                throw new LoaderException($"Could not find attribute '{attributeName}' for xpath '{xpath}' in entity {entityName}.");
            }

            return attribute;
        }
    }
}
