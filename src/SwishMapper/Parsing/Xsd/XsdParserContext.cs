
using System.Collections.Generic;
using System.Xml.Schema;

using SwishMapper.Models;

namespace SwishMapper.Parsing.Xsd
{
    public class XsdParserContext
    {
        private readonly Dictionary<string, XsdElement> elements = new Dictionary<string, XsdElement>();
        private readonly XmlSchemaSet schemaSet;


        public XsdParserContext(XmlSchemaSet schemaSet)
        {
            this.schemaSet = schemaSet;

            elements = new Dictionary<string, XsdElement>();
        }


        private XsdParserContext(XsdParserContext parentContext)
        {
            schemaSet = parentContext.schemaSet;
            elements = parentContext.elements;
        }


        public int Depth { get; private set; }
        public XmlSchemaSet SchemaSet { get { return schemaSet; } }
        public IDictionary<string, XsdElement> Elements { get { return elements; } }


        public XsdParserContext Push(XsdElement element)
        {
            if (!elements.ContainsKey(element.Name))
            {
                elements.Add(element.Name, element);
            }

            var child = new XsdParserContext(this);

            child.Depth = Depth + 1;

            return child;
        }


        public bool HasElement(string name)
        {
            return elements.ContainsKey(name);
        }
    }
}
