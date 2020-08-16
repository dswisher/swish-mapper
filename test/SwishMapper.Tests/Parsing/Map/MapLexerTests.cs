
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
        [InlineData("examples")]
        [InlineData("ignore")]
        [InlineData("with")]
        public void CanLexKeyword(string word)
        {
            // Arrange
            using (var context = MakeWrapper(word))
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


        protected override LexerWrapper<MapLexer> MakeWrapper(string input)
        {
            return new MapLexerWrapper(input);
        }


        private class MapLexerWrapper : LexerWrapper<MapLexer>
        {
            public MapLexerWrapper(string input)
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
