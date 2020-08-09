
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapParser : IMapParser
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
                lexer.Advance();

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
                lexer.Consume(TokenKind.LeftCurly);

                while (lexer.Token.Kind != TokenKind.RightCurly)
                {
                    ParseStatement(context, lexer);
                }

                lexer.Consume(TokenKind.RightCurly);

                return;
            }

            // Constructs:
            //      ident = model(ident);
            //      ident:ident = ident:ident;
            var lhs = CompoundIdentifier.Parse(lexer);

            lexer.Consume(TokenKind.Equals);

            var rhs = expressionParser.Parse(lexer, context);

            // A little special handling for the model function
            if (rhs.FunctionName == "model")
            {
                var model = rhs.Arguments.First().Model;

                if (model == null)
                {
                    throw new ParserException("The 'model' function requires a model argument.");
                }

                context.AddModelAlias(lhs.Parts.First(), model);
            }
            else
            {
                var target = context.Resolve(lhs);

                var mapping = new ExpressiveMapping
                {
                    TargetAttribute = target,
                    Expression = rhs
                };

                context.MapList.Maps.Add(mapping);
            }

            lexer.Consume(TokenKind.Semicolon);
        }


        private void ParseWith(MapParserContext context, MapLexer lexer)
        {
            // with cident = cident ...statement...
            lexer.VerifyToken(TokenKind.Keyword, "with");

            lexer.Advance();

            var alias = lexer.Consume(TokenKind.Identifier);

            lexer.Consume(TokenKind.Equals);

            var rhs = CompoundIdentifier.Parse(lexer);

            // TODO - perform some sort of validation on the RHS - does it resolve to a model, etc

            context.Push(alias, rhs);

            ParseStatement(context, lexer);

            context.Pop();
        }
    }
}
