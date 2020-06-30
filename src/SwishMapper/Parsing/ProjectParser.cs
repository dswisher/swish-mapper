
using System;
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

            var projectFileInfo = new FileInfo(path);

            using (var reader = new StreamReader(projectFileInfo.FullName))
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
                            project.Sources.Add(ParseDoc<ProjectSource>(projectFileInfo, bits));
                            break;

                        case "sink":
                            project.Sinks.Add(ParseDoc<ProjectSink>(projectFileInfo, bits));
                            break;
                    }
                }
            }

            return project;
        }


        private T ParseDoc<T>(FileInfo projectFileInfo, string[] bits)
            where T : ProjectDocument
        {
            T doc = Activator.CreateInstance<T>();

            doc.Name = bits[1];
            doc.ProjectPath = bits[2];
            doc.FullPath = new FileInfo(Path.Combine(projectFileInfo.Directory.FullName, doc.ProjectPath)).FullName;

            doc.RootElementName = bits[3];

            return doc;
        }
    }
}
