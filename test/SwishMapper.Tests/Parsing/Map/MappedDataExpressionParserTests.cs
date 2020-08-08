
using System.IO;
using System.Linq;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using SwishMapper.Parsing.Map;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing.Map
{
    public class MappedDataExpressionParserTests
    {
        private readonly MappedDataExpressionParser parser;
        private readonly MapParserContext context;


        public MappedDataExpressionParserTests()
        {
            var project = ProjectBuilder.Predefined("input-output");

            context = new MapParserContext(project.Models);

            context.AddModelAlias("input", project.Models.FirstOrDefault(x => x.Id == "input"));

            parser = new MappedDataExpressionParser();
        }


        [Theory]
        [InlineData("input:Person/Name")]
        public void CanParseIdentifier(string content)
        {
            // Arrange
            using (var wrapper = new MapLexerWrapper(content))
            {
                // Act
                var expression = parser.Parse(wrapper.Lexer, context);

                // Assert
                expression.FunctionName.Should().BeNull();
                expression.Arguments.Should().HaveCount(1);

                var arg = expression.Arguments[0];

                arg.XPath.Should().Be("Person/Name");
            }
        }


        // TODO - xyzzy - put this into the mix!
#if false
        [Theory]
        [InlineData("model(input)", "model")]
        public void CanParseSingleArgumentFunctions(string content, string functionName)
        {
            // Arrange
            using (var wrapper = new MapLexerWrapper(content))
            {
                // Act
                var expression = parser.Parse(wrapper.Lexer, context);

                // Assert
                expression.FunctionName.Should().Be(functionName);

                // TODO - check arguments
            }
        }
#endif


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