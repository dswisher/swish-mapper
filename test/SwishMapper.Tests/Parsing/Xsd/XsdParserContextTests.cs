
using FluentAssertions;
using SwishMapper.Models;
using SwishMapper.Parsing.Xsd;
using Xunit;

namespace SwishMapper.Tests.Parsing.Xsd
{
    public class XsdParserContextTests
    {
        private readonly XsdParserContext rootContext = new XsdParserContext(null);
        private readonly XsdElement rootElement = new XsdElement("rootElement");
        private readonly XsdElement childElement = new XsdElement("childElement");


        [Fact]
        public void InitialDepthIsZero()
        {
            rootContext.Depth.Should().Be(0);
        }


        [Fact]
        public void PushingIncrementsDepth()
        {
            // Act
            var childContext = rootContext.Push(rootElement);

            // Assert
            childContext.Depth.Should().Be(1);
        }


        [Fact]
        public void ElementsAreSharedWithChildren()
        {
            // Act
            var childContext = rootContext.Push(rootElement);

            childContext.Push(childElement);

            // Assert
            rootContext.Elements.Keys.Should().Contain(childElement.Name);
        }
    }
}
