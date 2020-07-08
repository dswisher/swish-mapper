
using System.Collections.Generic;

namespace SwishMapper.Models.Project
{
    /// <summary>
    /// The definition of a data model, as parsed from a project file.
    /// </summary>
    public class ProjectModel
    {
        private readonly List<ProjectModelPopulator> populators = new List<ProjectModelPopulator>();

        /// <summary>
        /// The ID used for thie data model when referencing it in mappings and the like.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name used for this model in report.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of populators used to build up this data model.
        /// </summary
        public IList<ProjectModelPopulator> Populators { get { return populators; } }
    }
}
