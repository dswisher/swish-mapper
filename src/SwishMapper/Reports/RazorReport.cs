
using System.IO;
using System.Threading.Tasks;
using RazorEngine;
using RazorEngine.Templating;


namespace SwishMapper.Reports
{
    public abstract class RazorReport<T> : IReportWorker
    {
        private readonly string templateName;
        private readonly string outputPath;


        public RazorReport(string templateName, string outputPath)
        {
            this.templateName = templateName;
            this.outputPath = outputPath;
        }


        public abstract Task RunAsync();


        protected void CompileAndRun(T model)
        {
            // Run the report
            using (var writer = new StreamWriter(outputPath))
            {
                // service.Run(templateName, writer, typeof(T), model);
                Engine.Razor.Run(templateName, writer, typeof(T), model);
            }
        }
    }
}
