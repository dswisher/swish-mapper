
using Microsoft.Extensions.DependencyInjection;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Project;
using SwishMapper.Parsing.Xsd;
using SwishMapper.Reports;
using SwishMapper.Work;

namespace SwishMapper
{
    public static class ServiceExtensions
    {
        public static ServiceCollection UseMapper(this ServiceCollection services)
        {
            services.AddSingleton<ILexerFactory, LexerFactory>();
            services.AddSingleton<IMappingParser, MappingParser>();
            services.AddSingleton<IMappingProcessor, MappingProcessor>();
            services.AddSingleton<IProjectParser, ProjectParser>();
            services.AddSingleton<IProjectPlanner, ProjectPlanner>();
            services.AddSingleton<IReportPlanner, ReportPlanner>();
            services.AddSingleton<ITypeFactory, TypeFactory>();
            services.AddSingleton<IXsdParser, XsdParser>();

            services.AddTransient<CsvLoader>();
            services.AddTransient<DataModelAssembler>();
            services.AddTransient<DataProjectAssembler>();
            services.AddTransient<MapLoader>();
            services.AddTransient<ModelCleaner>();
            services.AddTransient<ModelMerger>();
            services.AddTransient<XsdLoader>();

            services.AddTransient<CopyEmbeddedWorker>();
            services.AddTransient<IndexPage>();
            services.AddTransient<MappingReport>();
            services.AddTransient<ModelReport>();

            return services;
        }
    }
}
