
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

            // TODO - create empty entries for all entities/attributes in the sink model

            foreach (var map in Mapping.Maps)
            {
                // The report is sink-oriented, so find the sink entity and attribute...
                var sinkEntity = model.FindOrCreateEntity(map.ToAttribute.Parent.Name);
                var sinkAttribute = sinkEntity.FindOrCreateAttribute(map.ToAttribute.Name);

                sinkAttribute.Maps.Add(map);
            }

            return model;
        }
    }
}
