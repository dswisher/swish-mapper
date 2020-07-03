
namespace SwishMapper.Models
{
    /// <summary>
    /// Common bits for sources and sinks.
    /// </summary>
    public abstract class ProjectDocument : ProjectFile
    {
        /// <summary>
        /// The name of this document, used to reference it in mappings and the like.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The root element of the source/sink, for those docs that require one.
        /// </summary>
        public string RootElementName { get; set; }
    }
}
