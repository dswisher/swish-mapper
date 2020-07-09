
namespace SwishMapper.Models.Data
{
    public class DataAttribute
    {
        /// <summary>
        /// The name of this attribute.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The data type of this attribute.
        /// </summary>
        // TODO - how to handle complex types? Perhaps this should be a class?
        public string DataType { get; set; }
    }
}
