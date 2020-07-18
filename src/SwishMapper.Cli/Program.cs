
using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorEngine.Templating;
using Serilog;
using SwishMapper.Models;
using SwishMapper.Parsing;
using SwishMapper.Work;

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

            var settings = new AppSettings
            {
                ProjectFilePath = new FileInfo(args[0]).FullName,
                ReportDir = new DirectoryInfo("OUTPUT").FullName,
                TempDir = new DirectoryInfo("SCRATCH").FullName
            };

            var services = new ServiceCollection();

            services.UseMapper();

            services.AddSingleton(new LoggerFactory().AddSerilog(Log.Logger));
            services.AddLogging(l => l.AddConsole());

            services.AddSingleton<App>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var app = serviceProvider.GetRequiredService<App>();

                try
                {
                    // TODO - add/use cancellation token!
                    app.RunAsync(settings).Wait();
                }
                catch (AggregateException ae)
                {
                    ae.Handle(ex =>
                    {
                        if (ex is ParserException)
                        {
                            if (ex.InnerException != null)
                            {
                                Log.Error(ex, "Unhandled parsing exception.");
                            }
                            else
                            {
                                Log.Error("{Type}: {Message}", ex.GetType().Name, ex.Message);
                            }

                            return true;
                        }
                        else if (ex is TypeException)
                        {
                            Log.Error("{Type}: {Message}", ex.GetType().Name, ex.Message);
                            return true;
                        }
                        else if (ex is TemplateCompilationException)
                        {
                            Log.Error("{Type}: {Message}", ex.GetType().Name, ex.Message);
                            return true;
                        }
                        else if (ex is ProjectPlannerException)
                        {
                            Log.Error("{Type}: {Message}", ex.GetType().Name, ex.Message);
                            return true;
                        }
                        else if (ex is LoaderException)
                        {
                            Log.Error("{Type}: {Message}", ex.GetType().Name, ex.Message);
                            return true;
                        }

                        return false;
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unhandled exception in main.");
                }
            }

            Log.CloseAndFlush();
        }
    }
}
