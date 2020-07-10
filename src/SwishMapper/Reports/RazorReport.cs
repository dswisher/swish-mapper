
using System.IO;
using System.Threading.Tasks;
using RazorEngine;
using RazorEngine.Templating;


namespace SwishMapper.Reports
{
    public abstract class RazorReport<T> : IReportWorker
    {
        private readonly string templateName;


        public RazorReport(string templateName)
        {
            this.templateName = templateName;
        }


        public string OutputPath { get; set; }


        public abstract Task RunAsync();


        protected void CompileAndRun(T model)
        {
            // Run the report
            using (var writer = new StreamWriter(OutputPath))
            {
                Engine.Razor.Run(templateName, writer, typeof(T), model);
            }
        }
    }
}
