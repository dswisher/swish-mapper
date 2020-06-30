
namespace SwishMapper.Models
{
    /// <summary>
    /// Common bits for sources and sinks.
    /// </summary>
    public abstract class ProjectDocument
    {
        /// <summary>
        /// The name of this document, used to reference it in mappings and the like.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The file containing the definition of the source/sink, as it appears in the project file.
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// The full path to the definition of this document.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// The root element of the source/sink, for those docs that require one.
        /// </summary>
        public string RootElementName { get; set; }
    }
}
