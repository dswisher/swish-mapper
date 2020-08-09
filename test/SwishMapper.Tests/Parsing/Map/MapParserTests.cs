
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models.Data;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Map;
using SwishMapper.Tests.TestHelpers;
using Xunit;


namespace SwishMapper.Tests.Parsing.Map
{
    public class MapParserTests
    {
        private readonly Mock<ILogger<MapLexer>> lexerLogger = new Mock<ILogger<MapLexer>>();
        private readonly Mock<ILogger<MapParser>> logger = new Mock<ILogger<MapParser>>();

        private readonly ILexerFactory factory;
        private readonly IMappedDataExpressionParser expressionParser;
        private readonly MapParser parser;

        public MapParserTests()
        {
            factory = new LexerFactory(null, lexerLogger.Object);
            expressionParser = new MappedDataExpressionParser();
            parser = new MapParser(factory, expressionParser, logger.Object);
        }


        [Fact]
        public async Task MissingFileThrows()
        {
            // Arrange
            Func<Task> act = async () => { await parser.ParseAsync("foo.map", null); };

            // Act and assert
            await act.Should().ThrowAsync<FileNotFoundException>();
        }


        [Theory]
        [InlineData("empty.map", "Empty")]
        [InlineData("comments.map", "Comments")]
        public async Task MapsWithoutMappingsArePointlessButParsable(string filename, string mapName)
        {
            // Arrange
            var path = FileFinder.FindMapFile(filename);

            // Act
            var map = await parser.ParseAsync(path.FullName, null);

            // Assert
            map.Maps.Should().BeEmpty();
            map.Name.Should().Be(mapName);
        }


        [Fact]
        public async Task ReferenceToUnknownModelThrows()
        {
            // Arrange
            var path = FileFinder.FindMapFile("bad-model.map");

            Func<Task> act = async () => { await parser.ParseAsync(path.FullName, Enumerable.Empty<DataModel>()); };

            // Act and assert
            await act.Should().ThrowAsync<ParserException>().WithMessage("*model*not found*");
        }


        [Theory]
        [InlineData("one-simple-map.map", "input-output", "People/FullName", "Person/Name")]
        [InlineData("one-curlied-scoped-map.map", "input-output", "People/FullName", "Person/Name")]
        [InlineData("one-naked-scoped-map.map", "input-output", "People/FullName", "Person/Name")]
        public async Task CanParseMapsWithMappings(string filename, string predefinedProjectName, string leftXPath, string rightXPath)
        {
            // Arrange
            var path = FileFinder.FindMapFile(filename);

            // Act
            var map = await parser.ParseAsync(path.FullName, ProjectBuilder.Predefined(predefinedProjectName).Models);

            // Assert
            map.Maps.Should().Contain(x => x.TargetAttribute.XPath == leftXPath);
            map.Maps.Should().Contain(x => x.Expression.Arguments.First().Attribute.XPath == rightXPath);

            map.Name.Should().NotBeEmpty();
        }
    }
}
