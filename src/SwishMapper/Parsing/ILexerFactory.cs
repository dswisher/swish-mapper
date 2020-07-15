
using SwishMapper.Parsing.Project;

namespace SwishMapper.Parsing
{
    public interface ILexerFactory
    {
        ProjectLexer CreateProjectLexer(string path);
    }
}
