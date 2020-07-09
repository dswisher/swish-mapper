
using System.Collections.Generic;

namespace SwishMapper.Models.Project
{
    /// <summary>
    /// The model of a project - the result of parsing a project file.
    /// </summary>
    public class ProjectDefinition
    {
        private readonly List<ProjectModel> models = new List<ProjectModel>();

        public string ReportPath { get; set; } = "OUTPUT";

        public IList<ProjectModel> Models { get { return models; } }
    }
}
