
using System;
using System.IO;
using System.Linq;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models;
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


        [Fact]
        public void UnknownDirectiveThrows()
        {
            // Arrange
            const string filename = "unknown-directive.sm";

            var path = FileFinder.FindProjectFile(filename).FullName;

            Action act = () => parser.Parse(path);

            // Act and assert
            act.Should().Throw<ParserException>()
                .Where(x => x.Filename == filename)
                .Where(x => x.LineNumber == 9);
        }


        [Theory]
        [InlineData("empty.sm")]
        [InlineData("comments.sm")]
        public void EmptyFileIsHandled(string name)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(name).FullName;

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
            var path = FileFinder.FindProjectFile(name).FullName;

            // Act
            var project = parser.Parse(path);

            // Assert
            project.Sources.Should().HaveCount(numSources);
            project.Sinks.Should().HaveCount(numSinks);
        }


        [Theory]
        [InlineData("one-mapping.sm", 1)]
        public void MappingsAreParsed(string name, int numMaps)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(name).FullName;

            // Act
            var project = parser.Parse(path);

            // Assert
            project.Mappings.Should().HaveCount(numMaps);
        }


        [Theory]
        [InlineData("one-source-and-sink.sm")]
        public void PathsAreRelativeToProjectFile(string name)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(name).FullName;

            // Act
            var project = parser.Parse(path);

            // Assert
            foreach (var doc in project.Sinks.Cast<ProjectDocument>().Concat(project.Sources))
            {
                var fullName = FileFinder.FindProjectFile(doc.ProjectPath).FullName;

                doc.FullPath.Should().Be(fullName);
            }
        }


        [Theory]
        [InlineData("one-source-and-sink.sm", "Source1", "SourceRoot")]
        [InlineData("one-source-and-sink.sm", "Sink1", "SinkRoot")]
        public void RootElementNamesAreParsed(string name, string docName, string expectedRootName)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(name).FullName;

            // Act
            var project = parser.Parse(path);

            // Assert
            bool fromSink = docName.Contains("sink", StringComparison.OrdinalIgnoreCase);

            var doc = fromSink ? (ProjectDocument)project.Sinks.First(x => x.Name == docName)
                : project.Sources.First(x => x.Name == docName);

            doc.RootElementName.Should().Be(expectedRootName);
        }
    }
}
