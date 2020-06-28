
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using SwishMapper.Parsing;
using Xunit;

namespace SwishMapper.Tests.Parsing
{
    public class XsdParserTests
    {
        private const string docName = "test-xsd";
        private const string rootElementName = "message";

        private readonly XsdParser parser;

        public XsdParserTests()
        {
            parser = new XsdParser();
        }


        [Fact]
        public async Task DocumentNameIsSet()
        {
            // Arrange
            var path = FindXsd("one-simple-element.xsd");   // any XSD would suffice

            // Act
            var doc = await parser.ParseAsync(path, docName, rootElementName, string.Empty);

            // Assert
            doc.Name.Should().Be(docName);
        }


        [Fact]
        public async Task DepthIsSet()
        {
            // arrange
            var path = FindXsd("nested-sequences.xsd");

            // Act
            var doc = await parser.ParseAsync(path, docName, rootElementName, string.Empty);

            // Assert
            doc.Elements.First(x => x.Name == "message").Depth.Should().Be(0);
            doc.Elements.First(x => x.Name == "payload").Depth.Should().Be(1);
        }


        [Theory]
        [InlineData("nested-sequences.xsd", new[] { "payload", "message" })]
        [InlineData("one-attribute.xsd", new[] { "message" })]
        [InlineData("one-child-element.xsd", new[] { "payload", "message" })]
        [InlineData("one-simple-element.xsd", new[] { "message" })]
        [InlineData("ref-element.xsd", new[] { "message", "payload" })]
        [InlineData("multiple-ref-elements.xsd", new[] { "message", "child1", "child2", "payload" })]
        public async Task ElementsAreTracked(string xsdName, string[] elementNames)
        {
            // Arrange
            var path = FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, docName, rootElementName, string.Empty);

            // Assert
            doc.Elements.Select(x => x.Name).Should().BeEquivalentTo(elementNames);
        }


        [Theory]
        [InlineData("one-simple-element.xsd", "message", "String" )]
        public async Task ElementDataTypesAreParsed(string xsdName, string elementName, string dataType)
        {
            // Arrange
            var path = FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, docName, rootElementName, string.Empty);

            // Assert
            var element = doc.Elements.First(x => x.Name == elementName);

            element.DataType.Should().Be(dataType);
        }


        [Theory]
        [InlineData("one-attribute.xsd", "message", "myAttribute", "String" )]
        [InlineData("ref-element.xsd", "payload", "myAttribute", "String" )]
        public async Task AttributeDataTypesAreParsed(string xsdName, string elementName, string attributeName, string dataType)
        {
            // Arrange
            var path = FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, docName, rootElementName, string.Empty);

            // Assert
            var element = doc.Elements.First(x => x.Name == elementName);
            var attribute = element.Attributes.First(x => x.Name == attributeName);

            attribute.DataType.Should().Be(dataType);
        }


        [Theory]
        [InlineData("one-child-element.xsd")]
        [InlineData("nested-sequences.xsd")]
        public async Task CanParseElementWithChildElement(string xsdName)
        {
            // Arrange
            var path = FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, docName, rootElementName, string.Empty);

            // Assert
            doc.RootElement.Elements.Should().HaveCount(1);

            var child = doc.RootElement.Elements[0];

            child.Name.Should().Be("payload");
            child.DataType.Should().Be("String");
            child.Depth.Should().Be(1);
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
