
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Reports
{
    public class ModelReport : RazorReport<DataModel>
    {
        private readonly ILogger logger;

        public ModelReport(ILogger<ModelReport> logger)
            : base("model-report")
        {
            this.logger = logger;
        }


        public DataModel Model { get; set; }


        public override Task RunAsync()
        {
            logger.LogInformation("Writing ModelReport for {Name} to {Path}.", Model.Name, OutputPath);

            CompileAndRun(Model);

            return Task.CompletedTask;
        }
    }
}
