
using System;
using System.IO;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Parsing;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing
{
    public class ProjectParserTests
    {
        private readonly Mock<ILogger<ProjectParser>> logger = new Mock<ILogger<ProjectParser>>();

        private readonly ProjectParser parser;

        public ProjectParserTests()
        {
            parser = new ProjectParser(logger.Object);
        }


        [Fact]
        public void MissingFileThrows()
        {
            // Arrange
            Action act = () => parser.Parse("foo.sm");

            // Act and assert
            act.Should().Throw<FileNotFoundException>();
        }


        [Theory]
        [InlineData("empty.sm")]
        [InlineData("comments.sm")]
        public void EmptyFileIsHandled(string name)
        {
            // Arrange
            var path = FileFinder.FindProject(name);

            // Act
            var project = parser.Parse(path);

            // Assert
            project.Should().NotBeNull();
        }


        [Theory]
        [InlineData("one-source-and-sink.sm", 1, 1)]
        public void SourcesAndSinksAreParsed(string name, int numSources, int numSinks)
        {
            // Arrange
            var path = FileFinder.FindProject(name);

            // Act
            var project = parser.Parse(path);

            // Assert
            project.Sources.Should().HaveCount(numSources);
            project.Sinks.Should().HaveCount(numSinks);
        }


        // TODO - relative paths should be relative to the project file
        // TODO - sources and sinks need to specify the root element, at least for a DTD
    }
}
