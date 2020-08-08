
using System.Collections.Generic;
using System.IO;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using SwishMapper.Parsing.Map;
using Xunit;

namespace SwishMapper.Tests.Parsing.Map
{
    public class CompoundIdentifierTests
    {
        [Theory]
        [MemberData(nameof(Identifiers))]
        public void CanConstructThemAll(string content)
        {
            // Arrange and Act
            var ident = new CompoundIdentifier(content);

            // Assert
            ident.ToString().Should().Be(content);
        }


        [Theory]
        [MemberData(nameof(Identifiers))]
        public void CanParseThemAll(string content)
        {
            // Arrange
            using (var context = new MapLexerWrapper(content))
            {
                // Act
                var ident = CompoundIdentifier.Parse(context.Lexer);

                // Assert
                ident.ToString().Should().Be(content);
            }
        }


        [Theory]
        [InlineData("name")]
        [InlineData("pre:name")]
        public void PartsAreSet(string content)
        {
            // Arrange
            using (var context = new MapLexerWrapper(content))
            {
                // Act
                var ident = CompoundIdentifier.Parse(context.Lexer);

                // Assert
                if (!content.Contains(":"))
                {
                    ident.Prefix.Should().BeNull();
                }

                ident.Parts.Should().BeEquivalentTo("name");
            }
        }


        [Theory]
        [InlineData("pre:name")]
        [InlineData("pre:part1/part2")]
        public void PrefixIsSet(string content)
        {
            // Arrange
            using (var context = new MapLexerWrapper(content))
            {
                // Act
                var ident = CompoundIdentifier.Parse(context.Lexer);

                // Assert
                ident.Prefix.Should().Be("pre");
            }
        }


        public static IEnumerable<object[]> Identifiers()
        {
            yield return new object[] { "name" };
            yield return new object[] { "part1/part2" };
            yield return new object[] { "part1/part2/part3" };
            yield return new object[] { "pre:name" };
            yield return new object[] { "pre:part1/part2/part3" };
        }


        private class MapLexerWrapper : LexerWrapper<MapLexer>
        {
            public MapLexerWrapper(string input)
                : base(input)
            {
                // Always advance to the first token
                Lexer.LexToken();
            }

            protected override MapLexer MakeLexer(StreamReader reader, string filename, ILogger<MapLexer> logger)
            {
                return new MapLexer(reader, filename, logger);
            }
        }
    }
}
