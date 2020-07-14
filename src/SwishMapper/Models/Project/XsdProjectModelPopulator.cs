
namespace SwishMapper.Models.Project
{
    /// <summary>
    /// The definition (details) needed to populate a model from an XSD file.
    /// </summary>
    public class XsdProjectModelPopulator : ProjectModelPopulator
    {
        /// <summary>
        /// The path to the file from which model info will be parsed.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The entity that is the starting point of the data model.
        /// </summary>
        public string RootEntity { get; set; }      // TODO - eliminate this
    }
}
