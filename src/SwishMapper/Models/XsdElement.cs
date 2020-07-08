
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// A data item that consists of other data items.
    /// </summary>
    public class XsdElement : XsdItem
    {
        private readonly List<XsdElement> elements = new List<XsdElement>();
        private readonly List<XsdAttribute> attributes = new List<XsdAttribute>();

        public XsdElement(string name)
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
        public IList<XsdAttribute> Attributes { get { return attributes; } }

        /// <summary>
        /// The child elements of this element.
        /// </summary>
        public IList<XsdElement> Elements { get { return elements; } }
    }
}
