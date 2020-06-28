
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public class XsdParser
    {
        public Task<DataDocument> ParseAsync(string path, string docName, string rootElementName, string rootElementNamespace)
        {
            // Load the XSD into a schema set and compile it up
            var schemaSet = new XmlSchemaSet();

            // TODO - handle xsd:include, see https://github.com/dswisher/swish-mapper/issues/1

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = XmlReader.Create(stream))
            {
                schemaSet.Add(null, reader);
            }

            schemaSet.Compile();

            // Find the root element, wrap it, and begin walking
            var qualifiedRoot = new XmlQualifiedName(rootElementName, rootElementNamespace);

            var xsdRoot = (XmlSchemaElement)schemaSet.GlobalElements[qualifiedRoot];

            var context = new ParserContext(schemaSet);

            var rootElement = Walk(xsdRoot, context);

            var doc = new DataDocument
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


        private DataElement Walk(XmlSchemaElement xsdElement, ParserContext context)
        {
            // TODO - for local elements, build the name based on the name of the parent
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

            var element = new DataElement(xsdElement.Name);

            element.Depth = context.Depth;

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


        private void Walk(DataElement element, XmlSchemaComplexType complexType, ParserContext context)
        {
            // Pick out any attributes, and add them to the element
            foreach (XmlSchemaAttribute xsdAttribute in complexType.Attributes)
            {
                var attribute = new DataAttribute(xsdAttribute.Name);

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


        private void Walk(DataElement element, XmlSchemaSimpleType simpleType, ParserContext context)
        {
            element.DataType = simpleType.Datatype.ValueType.Name;
        }


        private void Walk(DataElement element, XmlSchemaSequence sequence, ParserContext context)
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
