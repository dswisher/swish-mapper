
namespace SwishMapper.Models
{
    /// <summary>
    /// Common properties for attributes and elements.
    /// </summary>
    public abstract class DataItem
    {
        protected DataItem(string name)
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
    }
}
