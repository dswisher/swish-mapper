
namespace SwishMapper.Models
{
    /// <summary>
    /// A data item that contains a primitive type (string, integer, etc).
    /// </summary>
    /// <remarks>
    /// Within an XSD or XML document, this could be an attribute or an element with a primitive type.
    /// </remarks>
    public class DataAttribute
    {
        public DataAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of this attribute.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The data type of this attribute.
        /// </summary>
        public string DataType { get; set; }
    }
}
