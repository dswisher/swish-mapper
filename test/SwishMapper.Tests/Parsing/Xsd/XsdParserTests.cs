
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models;
using SwishMapper.Parsing.Xsd;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing.Xsd
{
    public class XsdParserTests
    {
        private readonly Mock<ILogger<XsdParser>> logger = new Mock<ILogger<XsdParser>>();

        private readonly XsdParser parser;

        public XsdParserTests()
        {
            parser = new XsdParser(logger.Object);
        }


        [Theory]
        [InlineData("nested-sequences.xsd", new[] { "message" })]
        [InlineData("one-attribute.xsd", new[] { "message" })]
        [InlineData("one-child-element.xsd", new[] { "message" })]
        [InlineData("one-simple-element.xsd", new[] { "message" })]
        [InlineData("ref-element.xsd", new[] { "message", "payload" })]
        [InlineData("multiple-ref-elements.xsd", new[] { "message", "child1", "child2", "payload" })]
        [InlineData("child-element-with-type.xsd", new[] { "message", "Details" })]
        public async Task ElementsAreTracked(string xsdName, string[] elementNames)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path);

            // Assert
            doc.Elements.Select(x => x.Name).Should().Contain(elementNames);
        }


        [Theory]
        [InlineData("one-simple-element.xsd", "message", "String")]
        public async Task ElementDataTypesAreParsed(string xsdName, string elementName, string dataType)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path);

            // Assert
            var element = GetVerifiedElement(doc, elementName);

            element.DataType.Should().Be(dataType);
        }


        [Theory]
        [InlineData("one-attribute.xsd", "message", "myAttribute", "String", "1", "1")]
        [InlineData("ref-element.xsd", "payload", "myAttribute", "String", "0", "1")]
        [InlineData("multiple-ref-elements.xsd", "payload", "myAttribute", "String", "1", "1")]
        public async Task AttributeDataTypesAreParsed(string xsdName, string elementName, string attributeName, string dataType, string minOccurs, string maxOccurs)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path);

            // Assert
            var element = GetVerifiedElement(doc, elementName);
            var attribute = GetVerifiedAttribute(element, attributeName);

            attribute.DataType.Should().Be(dataType);
            attribute.MinOccurs.Should().Be(minOccurs);
            attribute.MaxOccurs.Should().Be(maxOccurs);
        }


        [Theory]
        [InlineData("one-child-element.xsd", "message", "payload", "String")]
        [InlineData("nested-sequences.xsd", "message", "payload", "String")]
        [InlineData("ref-element.xsd", "message", "payload", "ref")]
        [InlineData("multiple-ref-elements.xsd", "message", "child1", "ref")]
        [InlineData("multiple-ref-elements.xsd", "message", "child2", "ref")]
        [InlineData("multiple-ref-elements.xsd", "child1", "payload", "ref")]
        [InlineData("multiple-ref-elements.xsd", "child2", "payload", "ref")]
        public async Task CanParseElementWithChildElement(string xsdName, string elementName, string childName, string dataType)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path);

            // Assert
            var parent = GetVerifiedElement(doc, elementName);
            var child = GetVerifiedElement(parent, childName);

            child.DataType.Should().Be(dataType);
        }


        [Theory]
        [InlineData("one-child-element.xsd", "message", "payload", "3", "37")]
        [InlineData("ref-element.xsd", "message", "payload", "1", "4")]
        public async Task MinOccursAndMaxOccursAreParsed(string xsdName, string parentName, string childName, string minOccurs, string maxOccurs)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path);

            // Assert
            var parent = GetVerifiedElement(doc, parentName);
            var child = GetVerifiedElement(parent, childName);

            child.MinOccurs.Should().Be(minOccurs);
            child.MaxOccurs.Should().Be(maxOccurs);
        }


        [Theory]
        [InlineData("ref-element.xsd", "message", "payload", "payload")]
        [InlineData("child-element-with-type.xsd", "message", "ThingDetails", "Details")]
        public async Task RefNamesAreProperlyParsed(string xsdName, string parentName, string childName, string refName)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path);

            // Assert
            var parent = GetVerifiedElement(doc, parentName);
            var child = GetVerifiedElement(parent, childName);

            child.DataType.Should().Be("ref");
            child.RefName.Should().Be(refName);
        }


        [Theory]
        [InlineData("nested-named-elements.xsd")]
        public async Task NestedNamedElementsAreSplitOut(string xsdName)
        {
            // Arrange
            var path = FileFinder.FindXsd(xsdName);

            // Act
            var doc = await parser.ParseAsync(path);

            // Assert
            // TODO
        }


        private XsdElement GetVerifiedElement(XsdDocument doc, string elementName)
        {
            doc.Elements.Should().Contain(x => x.Name == elementName);
            return doc.Elements.First(x => x.Name == elementName);
        }


        private XsdElement GetVerifiedElement(XsdElement parent, string elementName)
        {
            parent.Elements.Should().Contain(x => x.Name == elementName);
            return parent.Elements.First(x => x.Name == elementName);
        }


        private XsdAttribute GetVerifiedAttribute(XsdElement element, string attributeName)
        {
            element.Attributes.Should().Contain(x => x.Name == attributeName);
            return element.Attributes.First(x => x.Name == attributeName);
        }
    }
}
