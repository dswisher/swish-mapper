
namespace SwishMapper.Models.Old
{
    /// <summary>
    /// Common bits for sources and sinks.
    /// </summary>
    public abstract class ProjectFile
    {
        /// <summary>
        /// The file containing the definition of the file, as it appears in the project file.
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// The full path to the file.
        /// </summary>
        public string FullPath { get; set; }
    }
}
