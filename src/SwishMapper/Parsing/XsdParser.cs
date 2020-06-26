
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

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = XmlReader.Create(stream))
            {
                schemaSet.Add(null, reader);
            }

            schemaSet.Compile();

            // Find the root element, wrap it, and begin walking
            var qualifiedRoot = new XmlQualifiedName(rootElementName, rootElementNamespace);

            var xsdRoot = (XmlSchemaElement) schemaSet.GlobalElements[qualifiedRoot];

            var rootElement = Walk(xsdRoot);

            return Task.FromResult(new DataDocument
            {
                Name = docName,
                RootElement = rootElement
            });
        }


        private DataElement Walk(XmlSchemaElement xsdElement)
        {
            var name = xsdElement.Name;

            var element = new DataElement(xsdElement.Name);

            if (xsdElement.ElementSchemaType is XmlSchemaComplexType)
            {
                Walk(element, (XmlSchemaComplexType)xsdElement.ElementSchemaType);
            }
            else if (xsdElement.ElementSchemaType is XmlSchemaSimpleType)
            {
                Walk(element, (XmlSchemaSimpleType)xsdElement.ElementSchemaType);
            }
            else
            {
                throw new ParserException(xsdElement, $"Unexpected ElementSchemaType: {xsdElement.ElementSchemaType}.");
            }

            return element;
        }


        private void Walk(DataElement element, XmlSchemaComplexType complexType)
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
                Walk(element, (XmlSchemaSequence)complexType.Particle);
            }
            else
            {
                throw new ParserException(complexType, $"Unexpected complexType.Particle: {complexType.Particle}.");
            }
        }


        private void Walk(DataElement element, XmlSchemaSimpleType simpleType)
        {
            element.DataType = simpleType.Datatype.ValueType.Name;
        }


        private void Walk(DataElement element, XmlSchemaSequence sequence)
        {
            foreach (XmlSchemaAnnotated item in sequence.Items)
            {
                if (item is XmlSchemaElement)
                {
                    var child = Walk((XmlSchemaElement)item);

                    element.Elements.Add(child);
                }
                else
                {
                    throw new ParserException(sequence, $"Unexpected sequence item type: {item.GetType().Name}.");
                }
            }
        }
    }
}
