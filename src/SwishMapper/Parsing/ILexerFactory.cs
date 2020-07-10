
using SwishMapper.Parsing.Project;

namespace SwishMapper.Parsing
{
    public interface ILexerFactory
    {
        MappingLexer CreateMappingLexer(string path);
        ProjectLexer CreateProjectLexer(string path);
    }
}
