
using System.Collections.Generic;

using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MappedDataExpressionParser : IMappedDataExpressionParser
    {
        private readonly HashSet<string> functions = new HashSet<string>();

        public MappedDataExpressionParser()
        {
            // TODO - need more info on each function: arity, types, etc. For now, just names...
            functions.Add("model");
            functions.Add("concat");
        }


        public MappedDataExpression Parse(MapLexer lexer, MapParserContext context)
        {
            var expression = new MappedDataExpression();

            // Regardless, we should always start with an identifier. This means that functions
            // are not keywords.
            lexer.VerifyToken(TokenKind.Identifier);

            // If the identifier is a function name, parse a function call...
            if (functions.Contains(lexer.Token.Text))
            {
                // TODO
                throw new System.NotImplementedException();
            }
            else
            {
                // We don't have a function call, it must be an identifier. Parse it...
                var ident = CompoundIdentifier.Parse(lexer);

                expression.Arguments.Add(context.Resolve(ident));
            }

            return expression;
        }
    }
}
