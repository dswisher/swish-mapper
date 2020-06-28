
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// Represents a data source or data sink.
    /// </summary>
    /// <remarks>
    /// In some respects, this can be viewed as a simplified XSD.
    /// </remarks>
    public class DataDocument
    {
        private readonly Dictionary<string, DataElement> elements = new Dictionary<string, DataElement>();

        /// <summary>
        /// The name of this document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The top-most element in the document.
        /// </summary>
        public DataElement RootElement { get; set; }


        public IEnumerable<DataElement> Elements { get { return elements.Values; } }


        public void AddElement(DataElement element)
        {
            elements.Add(element.Name, element);
        }
    }
}
