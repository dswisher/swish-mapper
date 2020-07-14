
namespace SwishMapper.Models.Project
{
    /// <summary>
    /// The definition (details) about something that can populate a ProjectModel.
    /// </summary>
    public abstract class ProjectModelPopulator
    {
        /// <summary>
        /// The type of populator, used to determine which parser to use.
        /// </summary>
        public ProjectModelPopulatorType Type { get; set; }
    }
}
