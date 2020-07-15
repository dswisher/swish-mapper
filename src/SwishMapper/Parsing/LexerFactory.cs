
using Microsoft.Extensions.Logging;
using SwishMapper.Parsing.Project;

namespace SwishMapper.Parsing
{
    public class LexerFactory : ILexerFactory
    {
        private readonly ILogger<ProjectLexer> projectLogger;


        public LexerFactory(ILogger<ProjectLexer> projectLogger)
        {
            this.projectLogger = projectLogger;
        }


        public ProjectLexer CreateProjectLexer(string path)
        {
            return new ProjectLexer(path, projectLogger);
        }
    }
}
