
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Project;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing
{
    public class LexerFactoryTests
    {
        private readonly Mock<ILogger<MappingLexer>> mappingLogger = new Mock<ILogger<MappingLexer>>();
        private readonly Mock<ILogger<ProjectLexer>> projectLogger = new Mock<ILogger<ProjectLexer>>();

        private readonly LexerFactory factory;

        public LexerFactoryTests()
        {
            factory = new LexerFactory(mappingLogger.Object, projectLogger.Object);
        }


        [Fact]
        public void CanCreateMappingLexer()
        {
            // Arrange
            const string filename = "one.map";  // any valid mapping file works

            var path = FileFinder.FindMappingFile(filename).FullName;

            // Act
            var lexer = factory.CreateMappingLexer(path);

            // Assert
            lexer.Should().NotBeNull();
            lexer.Filename.Should().Be(filename);

            lexer.Dispose();
        }


        [Fact]
        public void CanCreateProjectLexer()
        {
            // Arrange - any valid project file works
            var path = FileFinder.FindProjectFile("one-xsd.sm").FullName;

            // Act
            var lexer = factory.CreateProjectLexer(path);

            // Assert
            lexer.Should().NotBeNull();
            lexer.FilePath.Should().Be(path);

            lexer.Dispose();
        }
    }
}
