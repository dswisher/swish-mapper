
using System.Collections.Generic;
using System.Xml.Schema;

using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    // TODO - rename to XsdParserContext
    public class ParserContext
    {
        private readonly Dictionary<string, DataElement> elements = new Dictionary<string, DataElement>();
        private readonly XmlSchemaSet schemaSet;


        public ParserContext(XmlSchemaSet schemaSet)
        {
            this.schemaSet = schemaSet;

            elements = new Dictionary<string, DataElement>();
        }


        private ParserContext(ParserContext parentContext)
        {
            schemaSet = parentContext.schemaSet;
            elements = parentContext.elements;
        }


        public int Depth { get; private set; }
        public XmlSchemaSet SchemaSet { get { return schemaSet; } }
        public IDictionary<string, DataElement> Elements { get { return elements; } }


        public ParserContext Push(DataElement element)
        {
            if (!elements.ContainsKey(element.Name))
            {
                elements.Add(element.Name, element);
            }

            var child = new ParserContext(this);

            child.Depth = Depth + 1;

            return child;
        }


        public bool HasElement(string name)
        {
            return elements.ContainsKey(name);
        }
    }
}
