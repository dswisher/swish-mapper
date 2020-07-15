
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
        private readonly Mock<ILogger<ProjectLexer>> projectLogger = new Mock<ILogger<ProjectLexer>>();

        private readonly LexerFactory factory;

        public LexerFactoryTests()
        {
            factory = new LexerFactory(projectLogger.Object);
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
