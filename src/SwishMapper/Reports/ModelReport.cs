
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Reports
{
    public class ModelReport : RazorReport<DataModel>
    {
        private readonly DataModel model;

        public ModelReport(DataModel model, string outputPath)
            : base("model-report", outputPath)
        {
            this.model = model;
        }


        public override Task RunAsync()
        {
            CompileAndRun(model);

            return Task.CompletedTask;
        }
    }
}
