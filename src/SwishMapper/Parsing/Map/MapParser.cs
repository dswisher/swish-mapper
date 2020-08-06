
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
        private readonly ILogger logger;

        public MapParser(ILexerFactory lexerFactory,
                         ILogger<MapParser> logger)
        {
            this.lexerFactory = lexerFactory;
            this.logger = logger;
        }


        public Task<ExpressiveMapList> ParseAsync(string path, IEnumerable<DataModel> models)
        {
            var mapping = new ExpressiveMapList();

            var context = new MapParserContext
            {
                MapList = mapping,
                Models = models
            };

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

            return Task.FromResult(mapping);
        }


        private void ParseStatement(MapParserContext context, MapLexer lexer)
        {
            // Constructs:
            //      ident = model(ident);
            //      ident:ident = ident:ident;
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

            var lhs = CompoundIdentifier.Parse(lexer);

            Consume(lexer, TokenKind.Equals);

            // TODO - xyzzy - right-hand-side parsing needs to be a more general expression

            if (lexer.Token.Kind == TokenKind.Keyword)
            {
                if (lexer.Token.Text == "model")
                {
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

            // TODO - xyzzy - process the "with" - add alias to context or w/e

            ParseStatement(context, lexer);
        }
    }
}
