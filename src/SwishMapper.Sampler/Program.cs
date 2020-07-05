
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SwishMapper.Sampling;

namespace SwishMapper.Sampler
{
    // TODO - merge this into SwishMapper.Cli, once improved option parsing support is available
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            // Validate arguments
            Options options = null;

            try
            {
                options = Options.Parse(args);
            }
            catch (OptionsException ex)
            {
                Log.Error(ex.Message);
                Log.Information(Options.Usage());
                return;
            }

            var services = new ServiceCollection();

            services.AddSingleton(new LoggerFactory().AddSerilog(Log.Logger));
            services.AddSingleton<App>();

            services.AddSingleton<ISampleStreamFinder, SampleStreamFinder>();
            services.AddSingleton<ISampleAccumulator, SampleAccumulator>();
            services.AddSingleton<IXmlSampler, XmlSampler>();

            services.AddLogging(l => l.AddConsole());

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var app = serviceProvider.GetRequiredService<App>();

                try
                {
                    // TODO - add/use cancellation token!
                    app.RunAsync(options).Wait();
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
