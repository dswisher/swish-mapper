
using Microsoft.Extensions.Logging;
using SwishMapper.Parsing.Map;
using SwishMapper.Parsing.Project;

namespace SwishMapper.Parsing
{
    public class LexerFactory : ILexerFactory
    {
        private readonly ILogger<ProjectLexer> projectLogger;
        private readonly ILogger<MapLexer> mapLogger;


        public LexerFactory(ILogger<ProjectLexer> projectLogger,
                            ILogger<MapLexer> mapLogger)
        {
            this.projectLogger = projectLogger;
            this.mapLogger = mapLogger;
        }


        public ProjectLexer CreateProjectLexer(string path)
        {
            return new ProjectLexer(path, projectLogger);
        }


        public MapLexer CreateMapLexer(string path)
        {
            return new MapLexer(path, mapLogger);
        }
    }
}
