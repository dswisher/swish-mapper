
using System.IO;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public class ProjectParser : IProjectParser
    {
        private readonly ILogger logger;

        public ProjectParser(ILogger<ProjectParser> logger)
        {
            this.logger = logger;
        }


        // TODO - change to async
        public ProjectDefinition Parse(string path)
        {
            var project = new ProjectDefinition();

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Skip comments and whitespace
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Split into words
                    var bits = line.Split();

                    switch (bits[0])
                    {
                        case "source":
                            project.Sources.Add(new ProjectSource { Path = bits[1] });
                            break;

                        case "sink":
                            project.Sinks.Add(new ProjectSink { Path = bits[1] });
                            break;
                    }
                }
            }

            return project;
        }
    }
}
