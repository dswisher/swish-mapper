
using Microsoft.Extensions.Logging;

namespace SwishMapper.Parsing
{
    public class LexerFactory : ILexerFactory
    {
        private readonly ILogger<MappingLexer> mappingLogger;


        public LexerFactory(ILogger<MappingLexer> mappingLogger)
        {
            this.mappingLogger = mappingLogger;
        }


        public MappingLexer CreateMappingLexer(string path)
        {
            return new MappingLexer(path, mappingLogger);
        }
    }
}
