
namespace SwishMapper.Models.Project
{
    /// <summary>
    /// The definition of a mapping between two data models.
    /// </summary>
    public class ProjectMap
    {
        /// <summary>
        /// The ID of the source data model
        /// </summary>
        public string FromModelId { get; set; }

        /// <summary>
        /// The ID of the destination (sink) data model
        /// </summary>
        public string ToModelId { get; set; }

        /// <summary>
        /// The Path to the file that contains the actual mapping.
        /// </summary>
        public string Path { get; set; }
    }
}
