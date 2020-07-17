
using System.Collections.Generic;
using System.Linq;

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
        /// The attributes of this element.
        /// </summary>
        public IList<XsdAttribute> Attributes { get { return attributes; } }

        /// <summary>
        /// The child elements of this element.
        /// </summary>
        public IList<XsdElement> Elements { get { return elements; } }


        public XsdAttribute FindOrCreateAttribute(string name)
        {
            var attribute = attributes.FirstOrDefault(x => x.Name == name);

            if (attribute == null)
            {
                attribute = new XsdAttribute(name);

                attributes.Add(attribute);
            }

            return attribute;
        }


        public XsdElement FindOrCreateElement(string name)
        {
            var element = elements.FirstOrDefault(x => x.Name == name);

            if (element == null)
            {
                element = new XsdElement(name);

                elements.Add(element);
            }

            return element;
        }
    }
}
