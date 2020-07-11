
namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A source used to build the data model.
    /// </summary>
    public class DataModelSource
    {
        /// <summary>
        /// A short name for the source, unique only within its data model.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The full path of the file
        /// </summary>
        public string Path { get; set; }
    }
}
