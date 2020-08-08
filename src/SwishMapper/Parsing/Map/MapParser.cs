
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapParser : AbstractParser, IMapParser
    {
        private readonly ILexerFactory lexerFactory;
        private readonly IMappedDataExpressionParser expressionParser;
        private readonly ILogger logger;

        public MapParser(ILexerFactory lexerFactory,
                         IMappedDataExpressionParser expressionParser,
                         ILogger<MapParser> logger)
        {
            this.lexerFactory = lexerFactory;
            this.expressionParser = expressionParser;
            this.logger = logger;
        }


        public Task<ExpressiveMapList> ParseAsync(string path, IEnumerable<DataModel> models)
        {
            var context = new MapParserContext(models);

            using (var lexer = lexerFactory.CreateMapLexer(path))
            {
                // Move to the first token
                Advance(lexer);

                // Keep parsing until nothing is left.
                while (lexer.Token.Kind != TokenKind.EOF)
                {
                    ParseStatement(context, lexer);
                }
            }

            return Task.FromResult(context.MapList);
        }


        private void ParseStatement(MapParserContext context, MapLexer lexer)
        {
            // Constructs:
            //      with ...
            //      { statements }
            if (lexer.Token.Kind == TokenKind.Keyword)
            {
                if (lexer.Token.Text == "with")
                {
                    ParseWith(context, lexer);
                    return;
                }
                else
                {
                    throw new ParserException($"Unexpected keyword '{lexer.Token.Text}'.", lexer.Token);
                }
            }
            else if (lexer.Token.Kind == TokenKind.LeftCurly)
            {
                Consume(lexer, TokenKind.LeftCurly);

                while (lexer.Token.Kind != TokenKind.RightCurly)
                {
                    ParseStatement(context, lexer);
                }

                Consume(lexer, TokenKind.RightCurly);

                return;
            }

            // Constructs:
            //      ident = model(ident);
            //      ident:ident = ident:ident;
            var lhs = CompoundIdentifier.Parse(lexer);

            Consume(lexer, TokenKind.Equals);

            // TODO - xyzzy - proper RHS parsing!

#if false
            var rhs = expressionParser.Parse(lexer, context);

            // A little special handling for the model function
            if (rhs.FunctionName == "model")
            {
                var modelId = rhs.Arguments.First();
                var model = context.Models.FirstOrDefault(x => x.Id == modelId);

                if (model == null)
                {
                    throw new ParserException($"Model '{modelId}' not found in project.", lexer.Token);
                }

                context.AddModelAlias(lhs.Parts.First(), model);
            }
            else
            {
            }
#endif

#if true
            if (lexer.Token.Kind == TokenKind.Keyword)
            {
                if (lexer.Token.Text == "model")
                {
                    // Verify the LHS is legit
                    if (lhs.HasPrefix || lhs.Parts.Count() != 1)
                    {
                        throw new ParserException($"Model left-hand-side must be a simple identifier.", lexer.Token);
                    }

                    // Parse this simple expression
                    Consume(lexer, TokenKind.Keyword, "model");
                    Consume(lexer, TokenKind.LeftParen);

                    // Look up the model
                    var modelId = Consume(lexer, TokenKind.Identifier);
                    var model = context.Models.FirstOrDefault(x => x.Id == modelId);

                    if (model == null)
                    {
                        throw new ParserException($"Model '{modelId}' not found in project.", lexer.Token);
                    }

                    context.AddModelAlias(lhs.Parts.First(), model);

                    // ...and done...
                    Consume(lexer, TokenKind.RightParen);
                }
                else
                {
                    throw new ParserException($"Unknown function '{lexer.Token.Text}'.", lexer.Token);
                }
            }
            else
            {
                var rhs = CompoundIdentifier.Parse(lexer);

                // TODO - do something with this mapping!
                context.MapList.Maps.Add(new ExpressiveMapping());  // TODO - HACK!
            }
#endif

            Consume(lexer, TokenKind.Semicolon);
        }


        private void ParseWith(MapParserContext context, MapLexer lexer)
        {
            // with cident = cident ...statement...
            VerifyToken(lexer, TokenKind.Keyword, "with");

            Advance(lexer);

            var alias = Consume(lexer, TokenKind.Identifier);

            Consume(lexer, TokenKind.Equals);

            var rhs = CompoundIdentifier.Parse(lexer);

            // TODO - perform some sort of validation on the RHS - does it resolve to a model, etc

            context.Push(alias, rhs);

            ParseStatement(context, lexer);

            context.Pop();
        }
    }
}
