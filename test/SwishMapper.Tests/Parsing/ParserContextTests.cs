
using FluentAssertions;
using SwishMapper.Models;
using SwishMapper.Parsing;
using Xunit;

namespace SwishMapper.Tests.Parsing
{
    public class ParserContextTests
    {
        private readonly ParserContext rootContext = new ParserContext(null);
        private readonly DataElement rootElement = new DataElement("rootElement");
        private readonly DataElement childElement = new DataElement("childElement");


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
