
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;
using SwishMapper.Parsing.Map;

namespace SwishMapper.Work
{
    public class MapDslLoader : IMapDslLoader
    {
        private readonly IMapParser parser;
        private readonly ILogger logger;

        public MapDslLoader(IMapParser parser,
                            ILogger<MapDslLoader> logger)
        {
            this.parser = parser;
            this.logger = logger;
        }

        public string FromModelId { get; set; }
        public string ToModelId { get; set; }
        public string Path { get; set; }


        public async Task RunAsync(DataProject project)
        {
            logger.LogDebug("Loading DSL mapping file {Path}.", Path);

            // Parse the mapping document
            var dataMap = await parser.ParseAsync(Path, project.Models);

            // Add the mapping to the project
            project.ExpressiveMaps.Add(dataMap);
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0} -> {1}", FromModelId, ToModelId);
        }
    }
}
