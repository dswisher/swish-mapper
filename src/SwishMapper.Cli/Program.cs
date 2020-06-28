
using System;
using System.Xml.Schema;

using SwishMapper.Formatters;
using SwishMapper.Parsing;
using SwishMapper.Reports;

namespace SwishMapper.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("You must specify an XSD file and the name of the root element.");
                return;
            }

            var path = args[0];
            var rootName = args[1];

            var parser = new XsdParser();

            Console.WriteLine("Parsing {0}...", path);

            try
            {
                var doc = parser.ParseAsync(path, "test-xsd", rootName, string.Empty).Result;
                var rootElement = doc.RootElement;

                var report = new DocumentReport(doc);

                var formatter = new ConsoleFormatter();
                formatter.Write(report);
            }
            catch (ParserException ex)
            {
                Console.WriteLine("ParserException: {0}", ex.Message);
            }
            catch (XmlSchemaException ex)
            {
                Console.WriteLine("XmlSchemaException: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);
            }

            Console.WriteLine("Done.");
        }
    }
}
