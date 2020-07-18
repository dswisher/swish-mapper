
using System;
using System.IO;
using System.Linq;
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


        public Task<XsdDocument> ParseAsync(string path)
        {
            // Load the XSD into a schema set and compile it up
            var schemaSet = new XmlSchemaSet();

            AddSchemaToSet(schemaSet, path);

            schemaSet.Compile();

            // Create the doc
            var doc = new XsdDocument();

            // Set up the context we'll pass around to keep parameter lists shorter
            var context = new Context
            {
                SchemaSet = schemaSet,
                Document = doc
            };

            // Go through all the global elements and process them.
            foreach (XmlSchemaElement element in schemaSet.GlobalElements.Values)
            {
                try
                {
                    ProcessElement(context, doc, element);
                }
                catch (Exception ex)
                {
                    throw new ParserException(element, $"Unhandled exception parsing element: {element.Name}", ex);
                }
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


        private void ProcessElement(Context context, XsdDocument doc, XmlSchemaElement xmlSchemaElement)
        {
            var xsdElement = doc.FindOrCreateElement(xmlSchemaElement.Name);

            if (xmlSchemaElement.ElementSchemaType is XmlSchemaComplexType complex)
            {
                ProcessComplexType(context, xsdElement, complex);
            }
            else if (xmlSchemaElement.ElementSchemaType is XmlSchemaSimpleType simple)
            {
                ProcessSimpleType(xsdElement, simple);
            }
            else
            {
                throw new ParserException(xmlSchemaElement, $"Unexpected ElementSchemaType: {xmlSchemaElement.ElementSchemaType}.");
            }
        }


        private void ProcessComplexType(Context context, XsdElement xsdElement, XmlSchemaComplexType complexType)
        {
            // Pick out any attributes, and add them to the element
            foreach (XmlSchemaAttribute xmlSchemaAttribute in complexType.Attributes)
            {
                var xsdAttribute = xsdElement.FindOrCreateAttribute(xmlSchemaAttribute.Name);

                if (xmlSchemaAttribute.AttributeSchemaType is XmlSchemaSimpleType simple)
                {
                    ProcessSimpleType(xsdAttribute, simple);
                }
                else
                {
                    xsdAttribute.DataType = xmlSchemaAttribute.AttributeSchemaType.Datatype.ValueType.Name;
                }

                switch (xmlSchemaAttribute.Use)
                {
                    case XmlSchemaUse.None:
                    case XmlSchemaUse.Optional:
                        xsdAttribute.MinOccurs = "0";
                        xsdAttribute.MaxOccurs = "1";
                        break;

                    case XmlSchemaUse.Prohibited:
                        xsdAttribute.MinOccurs = "0";
                        xsdAttribute.MaxOccurs = "0";
                        break;

                    case XmlSchemaUse.Required:
                        xsdAttribute.MinOccurs = "1";
                        xsdAttribute.MaxOccurs = "1";
                        break;
                }
            }

            // Parse any children
            if (complexType.Particle == null)
            {
                // No children
                return;
            }
            else if (complexType.Particle is XmlSchemaSequence seq)
            {
                ProcessSequence(context, xsdElement, seq);
            }
            else
            {
                throw new ParserException(complexType, $"Unexpected complexType.Particle: {complexType.Particle}.");
            }
        }


        private void ProcessSimpleType(XsdItem xsdItem, XmlSchemaSimpleType simpleType)
        {
            xsdItem.DataType = simpleType.Datatype.ValueType.Name;

            var restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;
            if (restriction != null)
            {
                var maxLengthFacet = restriction.Facets.OfType<XmlSchemaMaxLengthFacet>().FirstOrDefault();
                if (maxLengthFacet != null)
                {
                    xsdItem.MaxLength = maxLengthFacet.Value;
                }
            }
        }


        private void ProcessSequence(Context context, XsdElement xsdElement, XmlSchemaSequence sequence)
        {
            foreach (XmlSchemaAnnotated item in sequence.Items)
            {
                if (item is XmlSchemaElement elem)
                {
                    ProcessChildElement(context, xsdElement, elem);
                }
                else if (item is XmlSchemaSequence seq)
                {
                    ProcessSequence(context, xsdElement, seq);
                }
                else
                {
                    throw new ParserException(sequence, $"Unexpected sequence item type: {item.GetType().Name}.");
                }
            }
        }


        private void ProcessChildElement(Context context, XsdElement xsdParent, XmlSchemaElement child)
        {
            XsdElement xsdChild;

            // If this is a reference element, set the type to reference
            var refName = child.RefName?.Name;
            if (!string.IsNullOrEmpty(refName))
            {
                xsdChild = xsdParent.FindOrCreateElement(refName);

                xsdChild.DataType = "ref";
                xsdChild.RefName = refName;
            }
            else
            {
                xsdChild = xsdParent.FindOrCreateElement(child.Name);

                if (child.ElementSchemaType is XmlSchemaComplexType complex)
                {
                    xsdChild.DataType = "ref";

                    if (!string.IsNullOrEmpty(child.SchemaTypeName.Name))
                    {
                        xsdChild.RefName = child.SchemaTypeName.Name;
                    }
                    else
                    {
                        xsdChild.RefName = child.Name;
                    }

                    // Only process if it does not already exist, to avoid stack overflow
                    if (context.Document.FindElement(xsdChild.RefName) == null)
                    {
                        var xsdTarget = context.Document.FindOrCreateElement(xsdChild.RefName);

                        ProcessComplexType(context, xsdTarget, complex);
                    }
                }
                else if (child.ElementSchemaType is XmlSchemaSimpleType simple)
                {
                    ProcessSimpleType(xsdChild, simple);
                }
                else
                {
                    throw new ParserException(child, $"Could not determine data type for child element: {xsdParent.Name}.{child.Name}.");
                }
            }

            xsdChild.MinOccurs = child.MinOccursString;
            xsdChild.MaxOccurs = child.MaxOccursString;
        }


        private class Context
        {
            public XsdDocument Document { get; set; }
            public XmlSchemaSet SchemaSet { get; set; }
        }
    }
}
