
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;
using SwishMapper.Parsing;
using Xunit;

namespace SwishMapper.Tests.Parsing
{
    public class XsdParserTests
    {
        private readonly XsdParser parser;

        public XsdParserTests()
        {
            parser = new XsdParser();
        }


        [Fact]
        public async Task CanParseDocumentWithOneSimpleElement()
        {
            // Arrange
            var path = FindXsd("one-simple-element.xsd");

            // Act
            var doc = await parser.ParseAsync(path, "test-xsd", "message", string.Empty);

            // Assert
            doc.Name.Should().Be("test-xsd");
            doc.RootElement.Should().NotBeNull();
            doc.RootElement.Name.Should().Be("message");
            doc.RootElement.DataType.Should().Be("String");
        }


        [Fact]
        public async Task CanParseElementWithOneAttribute()
        {
            // Arrange
            var path = FindXsd("one-attribute.xsd");

            // Act
            var doc = await parser.ParseAsync(path, "test-xsd", "message", string.Empty);

            // Assert
            doc.RootElement.Should().NotBeNull();
            doc.RootElement.Name.Should().Be("message");
            doc.RootElement.DataType.Should().BeNull();
            doc.RootElement.Attributes.Should().HaveCount(1);
            doc.RootElement.Attributes[0].Name.Should().Be("myAttribute");
            doc.RootElement.Attributes[0].DataType.Should().Be("String");
        }


        [Fact]
        public async Task CanParseElementWithChildElement()
        {
            // Arrange
            var path = FindXsd("one-child-element.xsd");

            // Act
            var doc = await parser.ParseAsync(path, "test-xsd", "message", string.Empty);

            // Assert
            doc.RootElement.Elements.Should().HaveCount(1);
            // TODO
        }


        private static string FindXsd(string name)
        {
            const string dataDirectory = "TestData";

            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while ((directory != null) && (directory.GetDirectories(dataDirectory).Length == 0))
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new DirectoryNotFoundException($"Could not find {dataDirectory} directory.");
            }

            directory = directory.GetDirectories(dataDirectory)[0];

            return Path.Join(directory.FullName, name);
        }
    }
}
