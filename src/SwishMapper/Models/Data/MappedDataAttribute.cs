
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class MappedDataAttribute
    {
        private Dictionary<string, string> examples = new Dictionary<string, string>();

        /// <summary>
        /// The attribute being mapped.
        /// </summary>
        public DataAttribute Attribute { get; set; }

        /// <summary>
        /// The xpath of the attribute within this mapping.
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// The examples for this attribute, if any.
        /// </summary>
        public IDictionary<string, string> Examples { get { return examples; } }
    }
}
