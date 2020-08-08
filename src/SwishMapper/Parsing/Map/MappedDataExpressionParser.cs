
using System.Collections.Generic;
using System.Linq;

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
                ParseFunctionCall(lexer, context, expression);
            }
            else
            {
                // We don't have a function call, it must be an identifier. Parse it...
                var ident = CompoundIdentifier.Parse(lexer);
                var resolved = context.Resolve(ident);

                expression.Arguments.Add(new MappedDataArgument(resolved));
            }

            return expression;
        }


        private void ParseFunctionCall(MapLexer lexer, MapParserContext context, MappedDataExpression expression)
        {
            // We're sitting on the function name...
            expression.FunctionName = lexer.Consume(TokenKind.Identifier);

            // Start of the argument list
            lexer.Consume(TokenKind.LeftParen);

            // We should have an identifier...
            // TODO - literals
            var ident = CompoundIdentifier.Parse(lexer);

            // The identifier is either an attribute or a model...
            if (ident.HasPrefix)
            {
                // An attribute...it needs at least two parts (entity and attribute names)...
                if (ident.Parts.Count() < 2)
                {
                    throw new ParserException(ident, $"Malformed identifier: '{ident}'.");
                }

                expression.Arguments.Add(new MappedDataArgument(context.Resolve(ident)));
            }
            else
            {
                // A model...look it up and add it...
                var modelId = ident.Parts.First();
                var model = context.Models.FirstOrDefault(x => x.Id == modelId);

                if (model == null)
                {
                    throw new ParserException(ident, $"Model '{modelId}' not found.");
                }

                expression.Arguments.Add(new MappedDataArgument(model));
            }

            // TODO - check for a comma to see if we have more arguments

            // End of the argument list
            lexer.Consume(TokenKind.RightParen);
        }
    }
}
