
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public class MappingParser : IMappingParser
    {
        private readonly ILexerFactory lexerFactory;
        private readonly ILogger logger;

        public MappingParser(ILexerFactory lexerFactory,
                             ILogger<MappingParser> logger)
        {
            this.lexerFactory = lexerFactory;
            this.logger = logger;
        }


        public Task<Mapping> ParseAsync(string path)
        {
            var mapFileInfo = new FileInfo(path);

            var mapping = new Mapping();

            using (var lexer = lexerFactory.CreateMappingLexer(mapFileInfo.FullName))
            {
                // Move to the first token
                lexer.LexToken();

                // Keep parsing until nothing is left.
                while (lexer.Token.Kind != TokenKind.EOF)
                {
                    ParseStatement(mapping, lexer);
                }
            }

            return Task.FromResult(mapping);
        }


        private void VerifyToken(MappingLexer lexer, TokenKind kind, string text = null)
        {
            if (lexer.Token.Kind != kind)
            {
                throw new ParserException($"Expecting token kind {kind}, but found {lexer.Token.Kind}.", lexer.Token);
            }

            if ((text != null) && (lexer.Token.Text != text))
            {
                throw new ParserException($"Expecting {kind} token to have text '{text}', but found '{lexer.Token.Text}'.", lexer.Token);
            }
        }


        private void ParseStatement(Mapping mapping, MappingLexer lexer)
        {
            VerifyToken(lexer, TokenKind.Keyword);

            switch (lexer.Token.Text)
            {
                case "sink":
                    ParseSink(mapping, lexer);
                    break;

                case "source":
                    ParseSource(mapping, lexer);
                    break;

                case "maps":
                    ParseMap(mapping, lexer);
                    break;

                default:
                    throw new ParserException($"Unexpected statement keyword: {lexer.Token.Text}.", lexer.Token);
            }
        }


        private string Consume(MappingLexer lexer, TokenKind kind, string text = null)
        {
            VerifyToken(lexer, kind, text);

            var result = lexer.Token.Text;

            lexer.LexToken();

            return result;
        }


        private void OptionallyConsume(MappingLexer lexer, TokenKind kind)
        {
            if (lexer.Token.Kind == kind)
            {
                lexer.LexToken();
            }
        }


        private void ParseSink(Mapping mapping, MappingLexer lexer)
        {
            Consume(lexer, TokenKind.Keyword, "sink");

            mapping.SinkName = Consume(lexer, TokenKind.Identifier);

            OptionallyConsume(lexer, TokenKind.Semicolon);
        }


        private void ParseSource(Mapping mapping, MappingLexer lexer)
        {
            Consume(lexer, TokenKind.Keyword, "source");

            mapping.AddSourceName(Consume(lexer, TokenKind.Identifier));

            OptionallyConsume(lexer, TokenKind.Semicolon);
        }


        private void ParseMap(Mapping mapping, MappingLexer lexer)
        {
            Consume(lexer, TokenKind.Keyword, "maps");
            Consume(lexer, TokenKind.LeftCurly);

            while (lexer.Token.Kind != TokenKind.RightCurly)
            {
                var sourceItem = Consume(lexer, TokenKind.Identifier);
                Consume(lexer, TokenKind.Arrow);
                var sinkItem = Consume(lexer, TokenKind.Identifier);

                mapping.AddEntry(new MappingEntry
                {
                    SourceItem = sourceItem,
                    SinkItem = sinkItem
                });

                OptionallyConsume(lexer, TokenKind.Semicolon);
            }

            Consume(lexer, TokenKind.RightCurly);
        }
    }
}
