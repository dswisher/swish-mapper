
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;

namespace SwishMapper.Parsing.Xsd
{
    public class XsdParser : IXsdParser
    {
        private readonly ILogger logger;

        public XsdParser(ILogger<XsdParser> logger)
        {
            this.logger = logger;
        }


        public Task<XsdDocument> ParseAsync(string path, string docName, string rootElementName, string rootElementNamespace)
        {
            // Load the XSD into a schema set and compile it up
            var schemaSet = new XmlSchemaSet();

            AddSchemaToSet(schemaSet, path);

            schemaSet.Compile();

            // Find the root element, wrap it, and begin walking
            var qualifiedRoot = new XmlQualifiedName(rootElementName, rootElementNamespace);

            var xsdRoot = (XmlSchemaElement)schemaSet.GlobalElements[qualifiedRoot];

            var context = new XsdParserContext(schemaSet);

            var rootElement = Walk(xsdRoot, context);

            var doc = new XsdDocument
            {
                Name = docName,
                RootElement = rootElement
            };

            foreach (var pair in context.Elements)
            {
                doc.AddElement(pair.Value);
            }

            return Task.FromResult(doc);
        }


        private void AddSchemaToSet(XmlSchemaSet set, string path)
        {
            logger.LogInformation("Loading XmlSchema from {Path}.", path);

            // Load the schema, and add it to the set.
            XmlSchema schema;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = XmlReader.Create(stream))
            {
                schema = XmlSchema.Read(reader, null);

                set.Add(schema);
            }

            // Process any includes that the schema might contain.
            foreach (XmlSchemaInclude include in schema.Includes)
            {
                var includePath = Path.Combine(Path.GetDirectoryName(path), include.SchemaLocation);

                AddSchemaToSet(set, includePath);
            }
        }


        private XsdElement Walk(XmlSchemaElement xsdElement, XsdParserContext context)
        {
            // TODO - do not merge local elements as if they are global - see https://github.com/dswisher/swish-mapper/issues/2
            var name = xsdElement.Name;

            if (name == null)
            {
                var refName = xsdElement.RefName;

                if (refName == null)
                {
                    throw new ParserException(xsdElement, $"Element name and ref are both null.");
                }

                var targetElement = context.SchemaSet.GlobalElements[refName];

                return Walk((XmlSchemaElement)targetElement, context);
            }

            if (context.HasElement(name))
            {
                return context.Elements[name];
            }

            var element = new XsdElement(xsdElement.Name);

            element.Depth = context.Depth;
            element.MinOccurs = xsdElement.MinOccursString;
            element.MaxOccurs = xsdElement.MaxOccursString;

            var childContext = context.Push(element);

            if (xsdElement.ElementSchemaType is XmlSchemaComplexType)
            {
                Walk(element, (XmlSchemaComplexType)xsdElement.ElementSchemaType, childContext);
            }
            else if (xsdElement.ElementSchemaType is XmlSchemaSimpleType)
            {
                Walk(element, (XmlSchemaSimpleType)xsdElement.ElementSchemaType, childContext);
            }
            else
            {
                throw new ParserException(xsdElement, $"Unexpected ElementSchemaType: {xsdElement.ElementSchemaType}.");
            }

            return element;
        }


        private void Walk(XsdElement element, XmlSchemaComplexType complexType, XsdParserContext context)
        {
            // Pick out any attributes, and add them to the element
            foreach (XmlSchemaAttribute xsdAttribute in complexType.Attributes)
            {
                var attribute = new XsdAttribute(xsdAttribute.Name);

                attribute.DataType = xsdAttribute.AttributeSchemaType.Datatype.ValueType.Name;

                // TODO - parse "use"

                element.Attributes.Add(attribute);
            }

            // Parse any children
            if (complexType.Particle == null)
            {
                // No children?
                return;
            }

            if (complexType.Particle is XmlSchemaSequence)
            {
                Walk(element, (XmlSchemaSequence)complexType.Particle, context);
            }
            else
            {
                throw new ParserException(complexType, $"Unexpected complexType.Particle: {complexType.Particle}.");
            }
        }


        private void Walk(XsdElement element, XmlSchemaSimpleType simpleType, XsdParserContext context)
        {
            element.DataType = simpleType.Datatype.ValueType.Name;
        }


        private void Walk(XsdElement element, XmlSchemaSequence sequence, XsdParserContext context)
        {
            foreach (XmlSchemaAnnotated item in sequence.Items)
            {
                if (item is XmlSchemaElement)
                {
                    var child = Walk((XmlSchemaElement)item, context);

                    element.Elements.Add(child);
                }
                else if (item is XmlSchemaSequence)
                {
                    Walk(element, (XmlSchemaSequence)item, context);
                }
                else
                {
                    throw new ParserException(sequence, $"Unexpected sequence item type: {item.GetType().Name}.");
                }
            }
        }
    }
}
