
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// Represents a data source or data sink.
    /// </summary>
    /// <remarks>
    /// In some respects, this can be viewed as a simplified XSD.
    /// </remarks>
    public class XsdDocument
    {
        private readonly Dictionary<string, XsdElement> elements = new Dictionary<string, XsdElement>();


        public IEnumerable<XsdElement> Elements { get { return elements.Values; } }


        public XsdElement FindOrCreateElement(string name)
        {
            if (elements.ContainsKey(name))
            {
                return elements[name];
            }

            var element = new XsdElement(name);

            elements.Add(name, element);

            return element;
        }


        public XsdElement FindElement(string name)
        {
            return elements.ContainsKey(name) ? elements[name] : null;
        }
    }
}
