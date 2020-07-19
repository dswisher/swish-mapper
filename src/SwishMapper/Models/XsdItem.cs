
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// Common properties for attributes and elements.
    /// </summary>
    public abstract class XsdItem
    {
        private readonly List<string> enumValues = new List<string>();

        protected XsdItem(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of this attribute/element.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type of data this attribute/element holds.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// For reference (ref) data types, this is the target of the reference
        /// </summary>
        public string RefName { get; set; }

        public string MinOccurs { get; set; }
        public string MaxOccurs { get; set; }

        public string MaxLength { get; set; }
        public string Comment { get; set; }

        public List<string> EnumValues { get { return enumValues; } }
    }
}
