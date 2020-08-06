
using System;
using System.IO;

using FluentAssertions;
using SwishMapper.Parsing;
using Xunit;

namespace SwishMapper.Tests.Parsing
{
    public abstract class AbstractLexerTests<T>
        where T : AbstractLexer, IDisposable
    {
        protected const string Filename = "memory-stream.map";


        [Fact]
        public void MissingFileThrows()
        {
            // Arrange
            Action act = () => MakeLexer();

            // Act and assert
            act.Should().Throw<FileNotFoundException>();
        }


        [Theory]
        [InlineData("&", 1, 1)]
        [InlineData(" &", 1, 2)]
        [InlineData("\n  &", 2, 3)]
        [InlineData(" - ", 1, 3)]
        [InlineData(" -* ", 1, 3)]
        public void UnexpectedCharacterThrows(string content, int line, int pos)
        {
            // Arrange
            using (var context = MakeWrapper(content))
            {
                Action act = () => context.Lexer.LexToken();

                // Act and assert
                act.Should().Throw<ParserException>()
                    .Where(x => x.LineNumber == line)
                    .Where(x => x.LinePosition == pos);
            }
        }


        [Theory]
        [InlineData("", TokenKind.EOF)]
        [InlineData("   ", TokenKind.EOF)]
        [InlineData("\n", TokenKind.EOF)]
        [InlineData("{", TokenKind.LeftCurly)]
        [InlineData("  {", TokenKind.LeftCurly)]
        [InlineData("  {  ", TokenKind.LeftCurly)]
        [InlineData("{  ", TokenKind.LeftCurly)]
        public void CanLexSingleToken(string content, TokenKind expectedKind)
        {
            // Arrange
            using (var context = MakeWrapper(content))
            {
                // Act
                context.Lexer.LexToken();
                var kind = context.Lexer.Token.Kind;

                context.Lexer.LexToken();
                var eof = context.Lexer.Token.Kind;

                // Assert
                kind.Should().Be(expectedKind);
                eof.Should().Be(TokenKind.EOF);
            }
        }


        [Theory]
        [InlineData(" { { ", TokenKind.LeftCurly, TokenKind.LeftCurly)]
        public void CanLexTwoTokens(string content, TokenKind expectedKind1, TokenKind expectedKind2)
        {
            // Arrange
            using (var context = MakeWrapper(content))
            {
                // Act
                context.Lexer.LexToken();
                var kind1 = context.Lexer.Token.Kind;

                context.Lexer.LexToken();
                var kind2 = context.Lexer.Token.Kind;

                context.Lexer.LexToken();
                var eof = context.Lexer.Token.Kind;

                // Assert
                kind1.Should().Be(expectedKind1);
                kind2.Should().Be(expectedKind2);
                eof.Should().Be(TokenKind.EOF);
            }
        }

        [Theory]
        [InlineData("source1")]
        [InlineData("camelCase")]
        [InlineData("PascalCase")]
        [InlineData("snake_case")]
        [InlineData("kebob-case")]
        public void CanLexIdentifier(string word)
        {
            // Arrange
            using (var context = MakeWrapper(word))
            {
                // Act
                context.Lexer.LexToken();

                // Assert
                var token = context.Lexer.Token;

                token.Kind.Should().Be(TokenKind.Identifier);
                token.Text.Should().Be(word);
            }
        }


        [Theory]
        [InlineData(" {  # a comment\n }")]
        [InlineData(" { } # a comment")]
        [InlineData(" { # a comment\n  # more comment\n }")]
        [InlineData("{# a comment\n# more comment\n}# yet more comment")]
        public void CommentsAreIgnored(string content)
        {
            // Arrange
            using (var context = MakeWrapper(content))
            {
                // Act and assert
                context.Lexer.LexToken();
                context.Lexer.Token.Kind.Should().Be(TokenKind.LeftCurly);

                context.Lexer.LexToken();
                context.Lexer.Token.Kind.Should().Be(TokenKind.RightCurly);

                context.Lexer.LexToken();
                context.Lexer.Token.Kind.Should().Be(TokenKind.EOF);
            }
        }


        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\"foo\"", "foo")]
        public void CanLexString(string content, string text)
        {
            // Arrange
            using (var context = MakeWrapper(content))
            {
                // Act and assert
                context.Lexer.LexToken();
                context.Lexer.Token.Kind.Should().Be(TokenKind.String);
                context.Lexer.Token.Text.Should().Be(text);
            }
        }


        // TODO - have each lexer-specific test supply a list of punctuation
        [Theory]
        [InlineData("{", TokenKind.LeftCurly)]
        [InlineData("}", TokenKind.RightCurly)]
        [InlineData(";", TokenKind.Semicolon)]
        [InlineData("->", TokenKind.Arrow)]
        [InlineData("=", TokenKind.Equals)]
        [InlineData("(", TokenKind.LeftParen)]
        [InlineData(")", TokenKind.RightParen)]
        [InlineData("/", TokenKind.Slash)]
        public void CanLexCommonPunctuation(string content, TokenKind expectedKind)
        {
            // Arrange
            using (var context = MakeWrapper(content))
            {
                // Act
                context.Lexer.LexToken();

                // Assert
                context.Lexer.Token.Kind.Should().Be(expectedKind);
            }
        }


        protected abstract T MakeLexer();
        protected abstract LexerWrapper<T> MakeWrapper(string input);
    }
}
