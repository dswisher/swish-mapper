
using System;
using System.Xml.Schema;
using SwishMapper.Parsing;

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
                var rootElement = parser.ParseAsync(path, "test-xsd", rootName, string.Empty).Result.RootElement;

                // TODO - print out the "tree"
                Console.WriteLine("Root element {0} has {1} attributes and {2} child elements.",
                    rootElement.Name, rootElement.Attributes.Count, rootElement.Elements.Count);
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
