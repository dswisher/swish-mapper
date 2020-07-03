
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models;
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

        private readonly List<DataDocument> sources = new List<DataDocument>();
        private readonly List<DataDocument> sinks = new List<DataDocument>();

        private readonly ILexerFactory factory;
        private readonly MappingParser parser;

        public MappingParserTests()
        {
            factory = new LexerFactory(lexerLogger.Object);
            parser = new MappingParser(factory, logger.Object);
        }


        [Fact]
        public async Task MissingFileThrows()
        {
            // Arrange
            Func<Task> act = async () => { await parser.ParseAsync("foo.map", sources, sinks); };

            // Act and assert
            await act.Should().ThrowAsync<FileNotFoundException>();
        }


        [Theory]
        [InlineData("unknown-directive.map")]
        public async Task UnknownDirectiveThrows(string name)
        {
            // Arrange
            var path = FileFinder.FindMappingFile(name);

            Func<Task> act = async () => { await parser.ParseAsync(path.FullName, sources, sinks); };

            // Act and assert
            (await act.Should().ThrowAsync<ParserException>())
                .Where(x => x.Filename == name)
                .Where(x => x.LineNumber == 2);
        }


        [Theory]
        [InlineData("unknown-sink.map")]
        [InlineData("unknown-source.map")]
        public async Task UnknownSinksAndSourcesThrow(string name)
        {
            // Arrange
            var path = FileFinder.FindMappingFile(name);

            Func<Task> act = async () => { await parser.ParseAsync(path.FullName, sources, sinks); };

            // Act and assert
            (await act.Should().ThrowAsync<ParserException>())
                .Where(x => x.Filename == name)
                .Where(x => x.LineNumber == 3);
        }


#if false
        [Fact]
        public async Task MappingsAreRead()
        {
            // Arrange
            // TODO - I don't like the coupling between the source/sink creation and the mapping file
            var path = FileFinder.FindMappingFile("one.map");

            // Set up the source
            var source = new DataDocument
            {
                Name = "src"
            };

            source.AddElement(new DataElement("source1"));

            // Set up the sink
            var sink = new DataDocument
            {
                Name = "snk"
            };

            sink.AddElement(new DataElement("sink1"));

            // Act
            var mapping = await parser.ParseAsync(path.FullName, new[] { source }, new[] { sink });

            // Assert
            // mapping.Entries.Should().HaveCount(1);

            // TODO - verify the lone mapping!
        }
#endif
    }
}
