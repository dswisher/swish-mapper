
using System;
using System.Xml.Schema;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SwishMapper.Formatters;
using SwishMapper.Parsing;
using SwishMapper.Reports;

namespace SwishMapper.Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // var providers = new LoggerProviderCollection();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                // .WriteTo.Providers(providers)
                .CreateLogger();

            // Validate arguments
            if (args.Length != 1)
            {
                Log.Error("You must specify a project file.");
                return;
            }

            var projectName = args[0];

            var services = new ServiceCollection();

            services.AddSingleton(new LoggerFactory().AddSerilog(Log.Logger));
            services.AddSingleton<IProjectParser, ProjectParser>();
            services.AddSingleton<App>();
            services.AddLogging(l => l.AddConsole());

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var app = serviceProvider.GetRequiredService<App>();

                app.Run(projectName);
            }

            Log.CloseAndFlush();

            // TODO - rip this out!
            // if (args.Length != 2)
            // {
            //     Console.WriteLine("You must specify an XSD file and the name of the root element.");
            //     return;
            // }

            // var path = args[0];
            // var rootName = args[1];

            // var parser = new XsdParser();

            // Console.WriteLine("Parsing {0}...", path);

            // try
            // {
            //     var doc = parser.ParseAsync(path, "test-xsd", rootName, string.Empty).Result;
            //     var rootElement = doc.RootElement;

            //     var report = new DocumentReport(doc);

            //     var formatter = new ConsoleFormatter(Console.Out);
            //     formatter.Write(report);
            // }
            // catch (ParserException ex)
            // {
            //     Console.WriteLine("ParserException: {0}", ex.Message);
            // }
            // catch (XmlSchemaException ex)
            // {
            //     Console.WriteLine("XmlSchemaException: {0}", ex.Message);
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine("Unhandled exception:");
            //     Console.WriteLine(ex);
            // }

            // Console.WriteLine("Done.");
        }
    }
}
