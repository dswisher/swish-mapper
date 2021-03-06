
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models.Project;
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
            factory = new LexerFactory(lexerLogger.Object, null);
            parser = new ProjectParser(factory, logger.Object);
        }


        [Fact]
        public async Task MissingFileThrows()
        {
            // Arrange
            Func<Task> act = async () => { await parser.ParseAsync("foo.sm"); };

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
        [InlineData("one-xsd.sm", "solo")]
        public async Task ModelIdIsParsed(string filename, string modelId)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(filename);

            // Act
            var project = await parser.ParseAsync(path.FullName);

            // Assert
            project.Models.Should().Contain(x => x.Id == modelId);
        }


        [Theory]
        [InlineData("one-xsd.sm", "my model")]
        public async Task ModelNameIsParsed(string filename, string modelName)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(filename);

            // Act
            var project = await parser.ParseAsync(path.FullName);

            // Assert
            project.Models.Should().Contain(x => x.Name == modelName);
        }


        [Theory]
        [InlineData("one-csv.sm", "solo", ProjectModelPopulatorType.Csv)]
        [InlineData("one-xsd.sm", "solo", ProjectModelPopulatorType.Xsd)]
        [InlineData("one-sample.sm", "solo", ProjectModelPopulatorType.Sample)]
        public async Task PopulatorsAreParsed(string filename, string modelId, ProjectModelPopulatorType type)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(filename);

            // Act
            var project = await parser.ParseAsync(path.FullName);

            // Assert
            var model = project.Models.First(x => x.Id == modelId);

            model.Populators.Should().Contain(x => x.Type == type);
        }


        [Theory]
        [InlineData("one-csv.sm", "solo", "bar/foo.csv")]
        [InlineData("one-xsd.sm", "solo", "bar/foo.xsd")]
        public async Task PopulatorPathsAreRelativeToProjectFile(string filename, string modelId, string fragment)
        {
            // Arrange
            var path = FileFinder.FindProjectFile(filename);

            var expected = Path.Combine(path.DirectoryName, fragment);

            // Act
            var project = await parser.ParseAsync(path.FullName);

            // Assert
            var populator = project.Models.First(x => x.Id == modelId).Populators.First();

            if (populator is CsvProjectModelPopulator)
            {
                ((CsvProjectModelPopulator)populator).Path.Should().Be(expected);
            }
            else if (populator is XsdProjectModelPopulator)
            {
                ((XsdProjectModelPopulator)populator).Path.Should().Be(expected);
            }
            else
            {
                true.Should().BeFalse();
            }
        }
    }
}
