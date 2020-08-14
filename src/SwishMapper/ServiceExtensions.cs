
using Microsoft.Extensions.DependencyInjection;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Map;
using SwishMapper.Parsing.Project;
using SwishMapper.Parsing.Xsd;
using SwishMapper.Reports;
using SwishMapper.Sampling;
using SwishMapper.Work;

namespace SwishMapper
{
    public static class ServiceExtensions
    {
        public static ServiceCollection UseMapper(this ServiceCollection services)
        {
            services.AddSingleton<ILexerFactory, LexerFactory>();
            services.AddSingleton<IProjectParser, ProjectParser>();
            services.AddSingleton<IProjectPlanner, ProjectPlanner>();
            services.AddSingleton<IReportPlanner, ReportPlanner>();
            services.AddSingleton<ITypeFactory, TypeFactory>();
            services.AddSingleton<IXsdParser, XsdParser>();

            services.AddTransient<ISampleAccumulator, SampleAccumulator>();
            services.AddTransient<ISampleStreamFinder, SampleStreamFinder>();
            services.AddTransient<IXmlSampler, XmlSampler>();

            services.AddTransient<IAttributeMerger, AttributeMerger>();
            services.AddTransient<ICsvNormalizer, CsvNormalizer>();
            services.AddTransient<ICsvToXsdTranslator, CsvToXsdTranslator>();
            services.AddTransient<IEmptyEntityCleaner, EmptyEntityCleaner>();
            services.AddTransient<IEntityMerger, EntityMerger>();
            services.AddTransient<IModelMerger, ModelMerger>();
            services.AddTransient<IModelSampleUpdater, ModelSampleUpdater>();
            services.AddTransient<IXsdToModelTranslator, XsdToModelTranslator>();
            services.AddTransient<IXsdLoader, XsdLoader>();
            services.AddTransient<IMapDslLoader, MapDslLoader>();
            services.AddTransient<IMapParser, MapParser>();
            services.AddTransient<IMapExampleLoader, MapExampleLoader>();
            services.AddTransient<IMappedDataExpressionParser, MappedDataExpressionParser>();

            // TODO - use interfaces for these!!!
            services.AddTransient<DataModelAssembler>();
            services.AddTransient<DataProjectAssembler>();
            services.AddTransient<SampleWriter>();

            services.AddTransient<CopyEmbeddedWorker>();
            services.AddTransient<IndexPage>();
            services.AddTransient<MappingReport>();
            services.AddTransient<ModelReport>();

            return services;
        }
    }
}
