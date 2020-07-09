
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorEngine.Templating;
using Serilog;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Project;
using SwishMapper.Reports;
using SwishMapper.Work;
using Old = SwishMapper.Parsing.Old;

namespace SwishMapper.Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            // Validate arguments
            // TODO - use a real argument parser library
            if (args.Length != 1)
            {
                Log.Error("You must specify a project file.");
                return;
            }

            var projectName = args[0];

            var services = new ServiceCollection();

            services.AddSingleton(new LoggerFactory().AddSerilog(Log.Logger));
            services.AddSingleton<ILexerFactory, LexerFactory>();
            services.AddSingleton<IMappingParser, MappingParser>();
            services.AddSingleton<IMappingProcessor, MappingProcessor>();
            services.AddSingleton<IProjectParser, ProjectParser>();
            services.AddSingleton<IProjectPlanner, ProjectPlanner>();
            services.AddSingleton<IReportPlanner, ReportPlanner>();
            services.AddSingleton<IXsdParser, XsdParser>();
            services.AddSingleton<App>();

            services.AddTransient<DataProjectAssembler>();
            services.AddTransient<DataModelAssembler>();
            services.AddTransient<XsdPopulator>();

            services.AddLogging(l => l.AddConsole());

            services.AddSingleton<Old.IProjectParser, Old.ProjectParser>();
            services.AddSingleton<OldApp>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                // var app = serviceProvider.GetRequiredService<OldApp>();
                var app = serviceProvider.GetRequiredService<App>();

                try
                {
                    // TODO - add/use cancellation token!
                    app.RunAsync(projectName).Wait();
                }
                catch (AggregateException ae)
                {
                    ae.Handle(ex =>
                    {
                        if (ex is ParserException)
                        {
                            Log.Error(ex.Message);
                            return true;
                        }
                        else if (ex is TemplateCompilationException)
                        {
                            Log.Error(ex.Message);
                            return true;
                        }

                        return false;
                    });
                }
                catch (Exception ex)
                {
                    // TODO - catch specific exceptions, like ParserException, and emit prettier messages
                    Log.Error(ex, "Unhandled exception in main.");
                }
            }

            Log.CloseAndFlush();
        }
    }
}
