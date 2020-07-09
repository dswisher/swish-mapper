
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorEngine.Templating;
using Serilog;
using SwishMapper.Parsing;

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
