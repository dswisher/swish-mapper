
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;
using SwishMapper.Models.Reports;

namespace SwishMapper.Reports
{
    public class MappingReport : RazorReport<MappingReportModel>
    {
        private readonly ILogger logger;

        public MappingReport(ILogger<MappingReport> logger)
            : base("mapping-report")
        {
            this.logger = logger;
        }


        public DataMapping Mapping { get; set; }


        public override Task RunAsync()
        {
            logger.LogInformation("Writing MappingReport to {Path}.", OutputPath);

            CompileAndRun(BuildModel());

            return Task.CompletedTask;
        }


        private MappingReportModel BuildModel()
        {
            var model = new MappingReportModel
            {
                Name = $"{Mapping.SourceModel.Name} -> {Mapping.SinkModel.Name}"
            };

            // Add the maps. For now, keep track of the entities...
            foreach (var map in Mapping.Maps)
            {
                // The report is sink-oriented, so find the sink entity and attribute...
                var sinkEntity = model.FindOrCreateEntity(map.ToAttribute.Parent.Name);
                var sinkAttribute = sinkEntity.FindOrCreateAttribute(map.ToAttribute.Name);

                // Add the map
                sinkAttribute.Maps.Add(map);

                // Add ALL attributes for this entity
                foreach (var attribute in map.ToAttribute.Parent.Attributes)
                {
                    sinkEntity.FindOrCreateAttribute(attribute.Name);
                }
            }

            // Return what we've build
            return model;
        }
    }
}
