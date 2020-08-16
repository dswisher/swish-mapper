
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        viewEntity.FindOrCreateAttribute(attribute);
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

                foreach (var note in map.Notes)
                {
                    viewAttribute.Notes.Add(new MappingReportNote
                    {
                        MapId = map.Id,
                        Note = note
                    });
                }

                BuildExamples(map, viewAttribute);

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


        private void BuildExamples(ExpressiveMapping map, MappingReportAttribute viewAttribute)
        {
            var builder = new StringBuilder();

            foreach (var attributeExample in map.TargetAttribute.Examples)
            {
                builder.Clear();

                var haveExpressionExample = BuildExampleExpression(builder, map.Expression, attributeExample.Key);

                // Build the example entry and add it
                viewAttribute.Examples.Add(new MappingReportExample
                {
                    MapId = map.Id,
                    ExampleId = attributeExample.Key,
                    Target = attributeExample.Value,
                    Expression = haveExpressionExample ? builder.ToString() : null
                });
            }
        }


        private bool BuildExampleExpression(StringBuilder builder, MappedDataExpression expression, string exampleId)
        {
            var found = false;
            var isFunction = !string.IsNullOrEmpty(expression.FunctionName);

            if (isFunction)
            {
                builder.Append(expression.FunctionName);
                builder.Append("(");
            }

            bool first = true;
            foreach (var arg in expression.Arguments)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(", ");
                }

                if (arg.Model != null)
                {
                    builder.Append(arg.Model.Id);
                }
                else if (arg.Attribute != null)
                {
                    if (arg.Attribute.Examples.ContainsKey(exampleId))
                    {
                        if (isFunction)
                        {
                            builder.Append("\"");
                        }

                        builder.Append(arg.Attribute.Examples[exampleId]);

                        if (isFunction)
                        {
                            builder.Append("\"");
                        }

                        found = true;
                    }
                    else
                    {
                        builder.Append("NIL");
                    }
                }
                else
                {
                    builder.Append("[unhandled-argument]");
                }
            }

            if (isFunction)
            {
                builder.Append(")");
            }

            return found;
        }
    }
}
