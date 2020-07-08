
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

        /// <summary>
        /// The name of this document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The top-most element in the document.
        /// </summary>
        public XsdElement RootElement { get; set; }


        public IEnumerable<XsdElement> Elements { get { return elements.Values; } }


        public void AddElement(XsdElement element)
        {
            elements.Add(element.Name, element);
        }
    }
}
