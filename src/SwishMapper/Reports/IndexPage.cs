
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Reports;

namespace SwishMapper.Reports
{
    public class IndexPage : RazorReport<IndexModel>
    {
        private readonly ILogger logger;


        public IndexPage(ILogger<IndexPage> logger)
            : base("index-page")
        {
            this.logger = logger;
        }


        public IndexModel Model { get; set; }


        public override Task RunAsync()
        {
            logger.LogInformation("Writing IndexPage to {Path}.", OutputPath);

            CompileAndRun(Model);

            return Task.CompletedTask;
        }
    }
}
