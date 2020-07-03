
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
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber += 1;

                    // Skip comments and whitespace
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Split into words
                    var bits = line.Split();

                    // Take the proper action
                    switch (bits[0])
                    {
                        case "mapping":
                            project.Mappings.Add((ProjectMapping)ParseFile(new ProjectMapping(), projectFileInfo, bits[1]));
                            break;

                        case "sink":
                            project.Sinks.Add(ParseDoc<ProjectSink>(projectFileInfo, bits));
                            break;

                        case "source":
                            project.Sources.Add(ParseDoc<ProjectSource>(projectFileInfo, bits));
                            break;

                        default:
                            // TODO - someday, keep track of line position (column) so it can be reported
                            throw new ParserException($"Unknown directive: '{bits[0]}'.", projectFileInfo.Name, lineNumber);
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

            ParseFile(doc, projectFileInfo, bits[2]);

            doc.RootElementName = bits[3];

            return doc;
        }


        private ProjectFile ParseFile(ProjectFile file, FileInfo projectFileInfo, string projectPath)
        {
            file.ProjectPath = projectPath;
            file.FullPath = new FileInfo(Path.Combine(projectFileInfo.Directory.FullName, projectPath)).FullName;

            return file;
        }
    }
}
