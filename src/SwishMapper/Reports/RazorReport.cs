
using System.IO;
using System.Threading.Tasks;

using RazorEngine.Configuration;
using RazorEngine.Templating;


namespace SwishMapper.Reports
{
    public abstract class RazorReport<T> : IReport
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
            // Set up the razor engine
            var config = new TemplateServiceConfiguration();

            config.TemplateManager = new EmbeddedResourceTemplateManager(typeof(ModelReport));

            var service = RazorEngineService.Create(config);

            // Compile the template
            var name = $"Templates.{templateName}";

            service.Compile(name, typeof(T));

            // Run the report
            using (var writer = new StreamWriter(outputPath))
            {
                service.Run(name, writer, typeof(T), model);
            }
        }
    }
}
