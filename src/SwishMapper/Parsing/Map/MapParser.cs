
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
        private readonly IMapExampleLoader exampleLoader;
        private readonly ILogger logger;

        public MapParser(ILexerFactory lexerFactory,
                         IMappedDataExpressionParser expressionParser,
                         IMapExampleLoader exampleLoader,
                         ILogger<MapParser> logger)
        {
            this.lexerFactory = lexerFactory;
            this.expressionParser = expressionParser;
            this.exampleLoader = exampleLoader;
            this.logger = logger;
        }


        public async Task<ExpressiveMapList> ParseAsync(string path, IEnumerable<DataModel> models)
        {
            // Parse the content
            var context = new MapParserContext(path, models);

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

            // Make sure we have a name
            if (string.IsNullOrEmpty(context.MapList.Name))
            {
                // TODO - need file name/line number here!
                throw new ParserException("Map must have a name.");
            }

            // Load the examples
            await exampleLoader.LoadAsync(context);

            // Return what we've got
            return context.MapList;
        }


        private void ParseStatement(MapParserContext context, MapLexer lexer)
        {
            // Constructs:
            //      examples ...
            //      with ...
            //      { statements }
            if (lexer.Token.Kind == TokenKind.Keyword)
            {
                if (lexer.Token.Text == "with")
                {
                    ParseWith(context, lexer);
                    return;
                }
                else if (lexer.Token.Text == "name")
                {
                    ParseName(context, lexer);
                    return;
                }
                else if (lexer.Token.Text == "examples")
                {
                    ParseExamples(context, lexer);
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

                lexer.Consume(TokenKind.Semicolon);
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

                if (lexer.Token.Kind == TokenKind.LeftCurly)
                {
                    ParseMappingExtras(context, lexer, mapping);
                }
                else
                {
                    lexer.Consume(TokenKind.Semicolon);
                }
            }
        }


        private void ParseMappingExtras(MapParserContext context, MapLexer lexer, ExpressiveMapping mapping)
        {
            // Consume the preamble
            lexer.Consume(TokenKind.LeftCurly);
            lexer.Consume(TokenKind.Keyword, "note");

            // Grab the note, and attach it to the mapping
            var note = lexer.Consume(TokenKind.String);

            mapping.Notes.Add(note);

            // Consume the postamble
            lexer.Consume(TokenKind.Semicolon);
            lexer.Consume(TokenKind.RightCurly);
        }


        private void ParseWith(MapParserContext context, MapLexer lexer)
        {
            // with cident = cident ...statement...
            lexer.Consume(TokenKind.Keyword, "with");

            var alias = lexer.Consume(TokenKind.Identifier);

            lexer.Consume(TokenKind.Equals);

            var rhs = CompoundIdentifier.Parse(lexer);

            if (string.IsNullOrEmpty(rhs.Prefix))
            {
                throw new ParserException(rhs, "Right-hand side of 'with' statement must have a prefix.");
            }

            // TODO - perform some sort of validation on the RHS - does it resolve to a model, etc

            context.Push(alias, rhs);

            ParseStatement(context, lexer);

            context.Pop();
        }


        private void ParseName(MapParserContext context, MapLexer lexer)
        {
            lexer.Consume(TokenKind.Keyword, "name");

            context.MapList.Name = lexer.Consume(TokenKind.String);

            lexer.Consume(TokenKind.Semicolon);
        }


        private void ParseExamples(MapParserContext context, MapLexer lexer)
        {
            // Build up the definition, so we can later load all the examples.
            var definition = new MapParserExamplesDefinition();

            // Parse the preamble
            lexer.Consume(TokenKind.Keyword, "examples");
            lexer.Consume(TokenKind.LeftCurly);

            // Keep parsing model blocks until we run out
            while (lexer.Token.Kind == TokenKind.Identifier || lexer.Token.Kind == TokenKind.Keyword)
            {
                // Grab the model name, and create a model object that we'll populate
                // TODO - create a new model example class thingy
                var modelId = lexer.Consume(TokenKind.Identifier, TokenKind.Keyword);
                lexer.Consume(TokenKind.LeftCurly);

                var model = new MapParserExamplesModel(modelId);
                definition.Models.Add(model);

                // Process model directives until we run out
                while (lexer.Token.Kind == TokenKind.Keyword)
                {
                    switch (lexer.Token.Text)
                    {
                        case "prefix":
                            lexer.Consume(TokenKind.Keyword, "prefix");
                            lexer.Consume(TokenKind.Colon);
                            model.Prefix = lexer.Consume(TokenKind.String);
                            lexer.Consume(TokenKind.Semicolon);
                            break;

                        case "suffix":
                            lexer.Consume(TokenKind.Keyword, "suffix");
                            lexer.Consume(TokenKind.Colon);
                            model.Suffix = lexer.Consume(TokenKind.String);
                            lexer.Consume(TokenKind.Semicolon);
                            break;

                        case "directory":
                            lexer.Consume(TokenKind.Keyword, "directory");
                            lexer.Consume(TokenKind.Colon);
                            model.Directories.Add(lexer.Consume(TokenKind.String));
                            lexer.Consume(TokenKind.Semicolon);
                            break;

                        default:
                            throw new ParserException($"Unexpected keyword in examples block: '{lexer.Token.Text}'.");
                    }
                }

                // Finish up the model block
                lexer.Consume(TokenKind.RightCurly);
            }

            // Finish up the examples block
            lexer.Consume(TokenKind.RightCurly);

            // Record what we found
            context.Examples.Add(definition);
        }
    }
}
