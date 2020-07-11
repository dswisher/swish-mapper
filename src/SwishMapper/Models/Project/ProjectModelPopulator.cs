
namespace SwishMapper.Models.Project
{
    public enum ProjectModelPopulatorType
    {
        Csv,
        Xsd
    }

    /// <summary>
    /// The definition (details) about something that can populate a ProjectModel.
    /// </summary>
    public class ProjectModelPopulator
    {
        /// <summary>
        /// The type of populator, used to determine which parser to use.
        /// </summary>
        public ProjectModelPopulatorType Type { get; set; }

        /// <summary>
        /// The path to the file from which model info will be parsed.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The entity that is the starting point of the data model, if applicable.
        /// </summary>
        /// <remarks>
        /// For example, in an XSD, this would be the root element.
        /// </remarks>
        public string RootEntity { get; set; }
    }
}
