
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

            // The report is sink-oriented, so go through and add all the entities/attributes
            // from the sink model. This will make it easy to see the mappings that are still lacking.
            foreach (var entity in Mapping.SinkModel.Entities)
            {
                var sinkEntity = model.FindOrCreateEntity(entity.Name);

                foreach (var attribute in entity.Attributes)
                {
                    var sinkAtt = sinkEntity.FindOrCreateAttribute(attribute.Name);

                    sinkAtt.SinkType = attribute.DataType;

                    // TODO - put this functionality in a Razor helper...
                    sinkAtt.Url = $"{Mapping.SinkModel.Id}.html#{entity.Name}";
                }
            }

            // Add the maps. For now, keep track of the entities...
            foreach (var map in Mapping.Maps)
            {
                // The report is sink-oriented, so find the sink entity and attribute, which should
                // have been created up above...
                var sinkEntity = model.FindEntity(map.ToAttribute.Parent.Name);
                var sinkAttribute = sinkEntity.FindAttribute(map.ToAttribute.Name);

                // Add the map
                sinkAttribute.Maps.Add(map);
            }

            // Return what we've build
            return model;
        }
    }
}
