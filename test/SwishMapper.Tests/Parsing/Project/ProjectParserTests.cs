
using System;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Project;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing.Project
{
    public class ProjectParserTests
    {
        private readonly Mock<ILogger<ProjectLexer>> lexerLogger = new Mock<ILogger<ProjectLexer>>();
        private readonly Mock<ILogger<ProjectParser>> logger = new Mock<ILogger<ProjectParser>>();

        private readonly ILexerFactory factory;
        private readonly ProjectParser parser;

        public ProjectParserTests()
        {
            factory = new LexerFactory(null, lexerLogger.Object);
            parser = new ProjectParser(factory, logger.Object);
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
        [InlineData("empty.sm")]
        [InlineData("comments.sm")]
        public async Task ProjectsWithoutModelsArePointlessButParsable(string filename)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(filename);

            // Act
            var project = await parser.ParseAsync(path.FullName);

            // Assert
            project.Models.Should().HaveCount(0);
        }


        [Theory]
        [InlineData("one-model.sm", "solo")]
        public async Task ModelIdIsParsed(string filename, string modelId)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(filename);

            // Act
            var project = await parser.ParseAsync(path.FullName);

            // Assert
            project.Models.Should().Contain(x => x.Id == modelId);
        }
    }
}
