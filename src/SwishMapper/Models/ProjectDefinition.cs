
using System.Collections.Generic;

namespace SwishMapper.Models
{
    public class ProjectDefinition
    {
        private readonly List<ProjectSource> sources = new List<ProjectSource>();
        private readonly List<ProjectSink> sinks = new List<ProjectSink>();

        public IList<ProjectSource> Sources { get { return sources; } }
        public IList<ProjectSink> Sinks { get { return sinks; } }
    }
}
