
using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;

namespace SwishMapper.Reports
{
    public static class ReportProviderExtensions
    {
        public static CopyEmbeddedWorker CopyEmbedded(this IServiceProvider provider, string filename, string outputDir)
        {
            var copy = provider.GetRequiredService<CopyEmbeddedWorker>();

            copy.SourcePath = $"Content.{filename}";
            copy.OutputPath = Path.Combine(outputDir, filename);

            return copy;
        }
    }
}
