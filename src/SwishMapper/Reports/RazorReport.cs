
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
        public string Title { get; set; }


        public abstract Task RunAsync();


        protected void CompileAndRun(T model)
        {
            // Run the report
            using (var writer = new StreamWriter(OutputPath))
            {
                DynamicViewBag bag = new DynamicViewBag();
                bag.AddValue("Title", Title);

                Engine.Razor.Run(templateName, writer, typeof(T), model, bag);
            }
        }
    }
}
