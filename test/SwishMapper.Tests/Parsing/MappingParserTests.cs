
using System;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Parsing;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing
{
    public class MappingParserTests
    {
        // private readonly Mock<ILexerFactory> factory = new Mock<ILexerFactory>();

        private readonly Mock<ILogger<MappingLexer>> lexerLogger = new Mock<ILogger<MappingLexer>>();
        private readonly Mock<ILogger<MappingParser>> logger = new Mock<ILogger<MappingParser>>();

        private readonly ILexerFactory factory;
        private readonly MappingParser parser;

        public MappingParserTests()
        {
            factory = new LexerFactory(lexerLogger.Object, null);
            parser = new MappingParser(factory, logger.Object);
        }


        [Fact]
        public async Task MissingFileThrows()
        {
            // Arrange
            Func<Task> act = async () => { await parser.ParseAsync("foo.map"); };

            // Act and assert
            await act.Should().ThrowAsync<FileNotFoundException>();
        }


        [Theory]
        [InlineData("unknown-directive.map")]
        public async Task UnknownDirectiveThrows(string name)
        {
            // Arrange
            var path = FileFinder.FindMappingFile(name);

            Func<Task> act = async () => { await parser.ParseAsync(path.FullName); };

            // Act and assert
            (await act.Should().ThrowAsync<ParserException>())
                .Where(x => x.Filename == name)
                .Where(x => x.LineNumber == 2);
        }


        [Theory]
        [InlineData("one.map", "snk")]
        [InlineData("one-semis.map", "snk")]
        public async Task SinkNameIsParsed(string filename, string sinkName)
        {
            // Arrange
            var path = FileFinder.FindMappingFile(filename);

            // Act
            var mapping = await parser.ParseAsync(path.FullName);

            // Assert
            mapping.SinkName.Should().Be(sinkName);
        }


        [Theory]
        [InlineData("one.map", new[] { "src" })]
        [InlineData("one-semis.map", new[] { "src" })]
        public async Task SourceNamesAreParsed(string filename, string[] sourceNames)
        {
            // Arrange
            var path = FileFinder.FindMappingFile(filename);

            // Act
            var mapping = await parser.ParseAsync(path.FullName);

            // Assert
            mapping.SourceNames.Should().BeEquivalentTo(sourceNames);
        }


        [Theory]
        [InlineData("one.map", 1, "source1", "sink1")]
        [InlineData("one-semis.map", 1, "source1", "sink1")]
        [InlineData("two.map", 2, "source2", "sink2")]
        [InlineData("two-semis.map", 2, "source2", "sink2")]
        public async Task MappingsAreParsed(string filename, int numEntries, string sampleSource, string sampleSink)
        {
            // Arrange
            var path = FileFinder.FindMappingFile(filename);

            // Act
            var mapping = await parser.ParseAsync(path.FullName);

            // Assert
            mapping.Entries.Should().HaveCount(numEntries);
            mapping.Entries.Should().Contain(x => x.SourceItem == sampleSource);
            mapping.Entries.Should().Contain(x => x.SinkItem == sampleSink);
        }


        // TODO - add test to verify only one sink can be specified
        // TODO - add test to verify a parse error is thrown if no sink or no sources are specified
    }
}
