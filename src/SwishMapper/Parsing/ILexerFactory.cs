
namespace SwishMapper.Parsing
{
    public interface ILexerFactory
    {
        MappingLexer CreateMappingLexer(string path);
    }
}
