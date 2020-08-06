
using System.IO;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Map;
using Xunit;

namespace SwishMapper.Tests.Parsing.Map
{
    public class MapLexerTests : AbstractLexerTests<MapLexer>
    {
        [Theory]
        [InlineData("with")]
        public void CanLexKeyword(string word)
        {
            // Arrange
            using (var context = MakeContext(word))
            {
                // Act
                context.Lexer.LexToken();

                // Assert
                var token = context.Lexer.Token;

                token.Kind.Should().Be(TokenKind.Keyword);
                token.Text.Should().Be(word);
            }
        }


        protected override MapLexer MakeLexer()
        {
            var logger = new Mock<ILogger<MapLexer>>();

            return new MapLexer(Filename, logger.Object);
        }


        protected override Context<MapLexer> MakeContext(string input)
        {
            return new MapContext(input);
        }


        private class MapContext : Context<MapLexer>
        {
            public MapContext(string input)
                : base(input)
            {
            }

            protected override MapLexer MakeLexer(StreamReader reader, string filename, ILogger<MapLexer> logger)
            {
                return new MapLexer(reader, filename, logger);
            }
        }
    }
}
