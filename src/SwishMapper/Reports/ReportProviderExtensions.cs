
using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using SwishMapper.Models.Data;
using SwishMapper.Models.Reports;

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


        public static ModelReport ModelReport(this IServiceProvider provider, DataModel model, string path)
        {
            var report = provider.GetRequiredService<ModelReport>();

            report.OutputPath = path;
            report.Model = model;

            return report;
        }


        public static MappingReport MappingReport(this IServiceProvider provider, DataMapping mapping, string path)
        {
            var report = provider.GetRequiredService<MappingReport>();

            report.OutputPath = path;
            report.Mapping = mapping;

            return report;
        }


        public static IndexPage IndexPage(this IServiceProvider provider, IndexModel model, string path)
        {
            var report = provider.GetRequiredService<IndexPage>();

            report.OutputPath = path;
            report.Model = model;

            return report;
        }
    }
}
