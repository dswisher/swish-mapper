
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models.Data;
using SwishMapper.Parsing.Map;
using Xunit;

namespace SwishMapper.Tests.Parsing.Map
{
    public class MapExampleXmlLoaderTests
    {
        private const string ModelId = "MyModel";
        private const string ExampleId = "MyExample";

        private readonly Mock<ILogger<MapExampleXmlLoader>> logger = new Mock<ILogger<MapExampleXmlLoader>>();

        private readonly MapExampleXmlLoader loader;

        public MapExampleXmlLoaderTests()
        {
            loader = new MapExampleXmlLoader(logger.Object);
        }


        [Fact]
        public void MissingNodeIsHandled()
        {
            // Arrange
            var attribute = new MappedDataAttribute
            {
                XPath = "Person/Name"
            };

            var attributes = new[] { attribute };
            var content = "<Person></Person>";

            // Act
            loader.LoadExamples(ModelId, content, attributes, ExampleId);

            // Assert
            attribute.Examples.Should().HaveCount(0);
        }


        [Theory]
        [InlineData("<Person><Name>Fred</Name></Person>", "Fred")]
        [InlineData("<Person><Name>Fred</Name><Name>Barney</Name></Person>", "1-of")]
        [InlineData("<Person><Name><First>Fred</First></Name></Person>", "non-terminal")]
        public void CanLoadOneExample(string content, string desired)
        {
            // Arrange
            var attribute = new MappedDataAttribute
            {
                XPath = "Person/Name"
            };

            var attributes = new[] { attribute };

            // Act
            loader.LoadExamples(ModelId, content, attributes, ExampleId);

            // Assert
            attribute.Examples.Should().HaveCount(1);

            attribute.Examples[ExampleId].Should().Contain(desired);
        }


        [Fact]
        public void CanLoadAttribute()
        {
            // Arrange
            var attribute = new MappedDataAttribute
            {
                XPath = "Person/@Name"
            };

            var attributes = new[] { attribute };
            var content = "<Person Name=\"Fred\"></Person>";

            // Act
            loader.LoadExamples(ModelId, content, attributes, ExampleId);

            // Assert
            attribute.Examples.Should().HaveCount(1);
        }
    }
}
