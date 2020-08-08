
namespace SwishMapper.Models.Data
{
    public class MappedDataAttribute
    {
        /// <summary>
        /// The attribute being mapped.
        /// </summary>
        public DataAttribute Attribute { get; set; }

        /// <summary>
        /// The xpath of the attribute within this mapping.
        /// </summary>
        public string XPath { get; set; }
    }
}
