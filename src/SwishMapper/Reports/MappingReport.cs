
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;
using SwishMapper.Models.Reports;

namespace SwishMapper.Reports
{
    public class MappingReport : RazorReport<MappingReportViewModel>
    {
        private readonly ILogger logger;

        public MappingReport(ILogger<MappingReport> logger)
            : base("mapping-report")
        {
            this.logger = logger;
        }


        public ExpressiveMapList Mapping { get; set; }


        public override Task RunAsync()
        {
            logger.LogInformation("Writing MappingReport to {Path}.", OutputPath);

            // TODO - refactor this so we can pass in a mock CompileAndRun
            CompileAndRun(BuildModel());

            return Task.CompletedTask;
        }


        private MappingReportViewModel BuildModel()
        {
            // Create the basic view model
            var reportViewModel = new MappingReportViewModel
            {
                Name = Mapping.Name
            };

            // To make it easy to see what hasn't yet been mapped, go through and add all the target
            // entities/attributes.
            foreach (var model in GetTargetModels())
            {
                var viewModel = reportViewModel.FindOrCreateModel(model);

                foreach (var entity in model.Entities)
                {
                    var viewEntity = viewModel.FindOrCreateEntity(entity);

                    foreach (var attribute in entity.Attributes)
                    {
                        var viewAttribute = viewEntity.FindOrCreateAttribute(attribute);

                        // TODO - populate URL??
                        // sinkAtt.Url = $"{Mapping.SinkModel.Id}.html#{entity.Name}";
                    }
                }
            }

            // Go through all the mappings, and stuff them into the proper place in the view
            foreach (var map in Mapping.Maps)
            {
                // Everything is based off the target attribute
                var dataAttribute = map.TargetAttribute.Attribute;

                // Build out the corresponding view objects...
                var viewModel = reportViewModel.FindOrCreateModel(dataAttribute.Parent.Parent);
                var viewEntity = viewModel.FindOrCreateEntity(dataAttribute.Parent);
                var viewAttribute = viewEntity.FindOrCreateAttribute(dataAttribute);

                viewAttribute.Maps.Add(map);
            }

            // Return what we've built
            return reportViewModel;
        }


        private IEnumerable<DataModel> GetTargetModels()
        {
            return Mapping.Maps
                .Select(x => x.TargetAttribute.Attribute.Parent.Parent)
                .Distinct();
        }
    }
}
