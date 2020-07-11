
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
            services.AddSingleton<IXsdParser, XsdParser>();

            services.AddTransient<DataProjectAssembler>();
            services.AddTransient<DataModelAssembler>();
            services.AddTransient<CsvPopulator>();
            services.AddTransient<XsdPopulator>();

            services.AddTransient<CopyEmbeddedWorker>();
            services.AddTransient<IndexPage>();
            services.AddTransient<ModelReport>();

            return services;
        }
    }
}
