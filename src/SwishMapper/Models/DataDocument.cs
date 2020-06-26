
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
        /// <summary>
        /// The name of this document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The top-most element in the document.
        /// </summary>
        public DataElement RootElement { get; set; }
    }
}
