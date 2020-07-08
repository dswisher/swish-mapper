
using System.Collections.Generic;

namespace SwishMapper.Models.Old
{
    public class ProjectDefinition
    {
        private readonly List<ProjectSource> sources = new List<ProjectSource>();
        private readonly List<ProjectSink> sinks = new List<ProjectSink>();
        private readonly List<ProjectMapping> mappings = new List<ProjectMapping>();

        public IList<ProjectSource> Sources { get { return sources; } }
        public IList<ProjectSink> Sinks { get { return sinks; } }
        public IList<ProjectMapping> Mappings { get { return mappings; } }
    }
}
