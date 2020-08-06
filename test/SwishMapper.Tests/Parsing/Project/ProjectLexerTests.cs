
using System.IO;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Project;
using Xunit;

namespace SwishMapper.Tests.Parsing.Project
{
    public class ProjectLexerTests : AbstractLexerTests<ProjectLexer>
    {
        [Theory]
        [InlineData("model")]
        [InlineData("xsd")]
        [InlineData("name")]
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


        protected override ProjectLexer MakeLexer()
        {
            var logger = new Mock<ILogger<ProjectLexer>>();

            return new ProjectLexer(Filename, logger.Object);
        }


        protected override LexerWrapper<ProjectLexer> MakeWrapper(string input)
        {
            return new ProjectLexerWrapper(input);
        }


        private class ProjectLexerWrapper : LexerWrapper<ProjectLexer>
        {
            public ProjectLexerWrapper(string input)
                : base(input)
            {
            }

            protected override ProjectLexer MakeLexer(StreamReader reader, string filename, ILogger<ProjectLexer> logger)
            {
                return new ProjectLexer(reader, filename, logger);
            }
        }
    }
}
