
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Parsing.Xsd;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing.Xsd
{
    public class XsdParserTests
    {
        private const string RootElementName = "message";

        private readonly Mock<ILogger<XsdParser>> logger = new Mock<ILogger<XsdParser>>();

        private readonly XsdParser parser;

        public XsdParserTests()
        {
            parser = new XsdParser(logger.Object);
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
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, RootElementName, string.Empty);

            // Assert
            doc.Elements.Select(x => x.Name).Should().BeEquivalentTo(elementNames);
        }


        [Theory]
        [InlineData("one-simple-element.xsd", "message", "String")]
        public async Task ElementDataTypesAreParsed(string xsdName, string elementName, string dataType)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, RootElementName, string.Empty);

            // Assert
            var element = doc.Elements.First(x => x.Name == elementName);

            element.DataType.Should().Be(dataType);
        }


        [Theory]
        [InlineData("one-attribute.xsd", "message", "myAttribute", "String")]
        [InlineData("ref-element.xsd", "payload", "myAttribute", "String")]
        public async Task AttributeDataTypesAreParsed(string xsdName, string elementName, string attributeName, string dataType)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, RootElementName, string.Empty);

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
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, RootElementName, string.Empty);

            // Assert
            doc.RootElement.Elements.Should().HaveCount(1);

            var child = doc.RootElement.Elements[0];

            child.Name.Should().Be("payload");
            child.DataType.Should().Be("String");
        }


        [Theory]
        [InlineData("one-child-element.xsd", "payload", "3", "37")]
        // TODO: [InlineData("ref-element.xsd", "payload", "1", "4")]
        public async Task MinOccursAndMaxOccursAreParsed(string xsdName, string elementName, string minOccurs, string maxOccurs)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path, RootElementName, string.Empty);

            // Assert
            var element = doc.Elements.First(x => x.Name == elementName);

            element.MinOccurs.Should().Be(minOccurs);
            element.MaxOccurs.Should().Be(maxOccurs);
        }
    }
}
