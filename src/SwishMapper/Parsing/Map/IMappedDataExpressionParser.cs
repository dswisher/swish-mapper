
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public interface IMappedDataExpressionParser
    {
        MappedDataExpression Parse(MapLexer lexer, MapParserContext context);
    }
}
