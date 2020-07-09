
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SwishMapper.Reports
{
    public class CopyEmbeddedWorker : IReportWorker
    {
        private readonly ILogger logger;
        private readonly Type rootType;
        private readonly Assembly assembly;


        public CopyEmbeddedWorker(ILogger<CopyEmbeddedWorker> logger)
        {
            this.logger = logger;

            rootType = GetType();
            assembly = rootType.Assembly;
        }


        public string SourcePath { get; set; }
        public string OutputPath { get; set; }


        public async Task RunAsync()
        {
            using (var input = assembly.GetManifestResourceStream(rootType, SourcePath))
            using (var output = new FileStream(OutputPath, FileMode.Create, FileAccess.Write))
            {
                await input.CopyToAsync(output);
            }
        }
    }
}
