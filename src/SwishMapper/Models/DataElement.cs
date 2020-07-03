
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// A data item that consists of other data items.
    /// </summary>
    public class DataElement : DataItem
    {
        private readonly List<DataElement> elements = new List<DataElement>();
        private readonly List<DataAttribute> attributes = new List<DataAttribute>();

        public DataElement(string name)
            : base(name)
        {
        }


        /// <summary>
        /// The depth of this element.
        /// </summary>
        /// <remarks>
        /// The root element has a depth of zero. If an element appears in multiple places in the
        /// hierarchy, this is the minimum value.
        /// </remarks>
        public int Depth { get; set; }

        /// <summary>
        /// The attributes of this element.
        /// </summary>
        public IList<DataAttribute> Attributes { get { return attributes; } }

        /// <summary>
        /// The child elements of this element.
        /// </summary>
        public IList<DataElement> Elements { get { return elements; } }
    }
}
