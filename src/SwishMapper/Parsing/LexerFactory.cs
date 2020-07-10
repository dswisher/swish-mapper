
using Microsoft.Extensions.Logging;
using SwishMapper.Parsing.Project;

namespace SwishMapper.Parsing
{
    public class LexerFactory : ILexerFactory
    {
        private readonly ILogger<MappingLexer> mappingLogger;
        private readonly ILogger<ProjectLexer> projectLogger;


        public LexerFactory(ILogger<MappingLexer> mappingLogger, ILogger<ProjectLexer> projectLogger)
        {
            this.mappingLogger = mappingLogger;
            this.projectLogger = projectLogger;
        }


        public MappingLexer CreateMappingLexer(string path)
        {
            return new MappingLexer(path, mappingLogger);
        }


        public ProjectLexer CreateProjectLexer(string path)
        {
            return new ProjectLexer(path, projectLogger);
        }
    }
}
