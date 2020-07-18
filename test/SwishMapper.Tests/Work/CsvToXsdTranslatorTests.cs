
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models.Data;
using SwishMapper.Parsing;
using SwishMapper.Tests.TestHelpers;
using SwishMapper.Work;
using Xunit;

namespace SwishMapper.Tests.Work
{
    public class CsvToXsdTranslatorTests
    {
        private const string Address = "Address";
        private const string Comment = "The best field ever.";

        private readonly Mock<ILogger<CsvToXsdTranslator>> logger = new Mock<ILogger<CsvToXsdTranslator>>();

        private readonly CsvToXsdTranslator translator;

        public CsvToXsdTranslatorTests()
        {
            translator = new CsvToXsdTranslator(logger.Object);
        }


        [Theory]
        [InlineData("required", "1", "1")]
        [InlineData("optional", "0", "1")]
        [InlineData(null, null, null)]
        public async Task CanLoadElementWithOneAttribute(string required, string minOccurs, string maxOccurs)
        {
            // Arrange
            translator.Input = new CsvRowBuilder()
                .Row()
                    .EntityName(Address)
                    .AttributeName("Id")
                    .DataType("string")
                    .MaxLength("12")
                    .Required(required)
                    .Comment(Comment);

            // Act
            var xsd = await translator.RunAsync();

            // Assert
            xsd.FindElement(Address).Should().NotBeNull();

            var element = xsd.FindElement(Address);

            element.Attributes.Should().Contain(x => x.Name == "Id");

            var attribute = element.Attributes.First(x => x.Name == "Id");

            attribute.DataType.Should().Be("string");
            attribute.MaxLength.Should().Be("12");
            attribute.MinOccurs.Should().Be(minOccurs);
            attribute.MaxOccurs.Should().Be(maxOccurs);
            attribute.Comment.Should().Be(Comment);
        }


        [Fact]
        public async Task CanLoadElementWithOneChild()
        {
            // Arrange
            translator.Input = new CsvRowBuilder()
                .Row()
                    .EntityName(Address)
                    .ChildElementName("City")
                    .DataType("ref")
                    .MinOccurs("2")
                    .MaxOccurs("23")
                .Row()
                    .EntityName("City")
                    .DataType("string")
                    .MaxLength("50");

            // Act
            var xsd = await translator.RunAsync();

            // Assert
            xsd.FindElement(Address).Should().NotBeNull();
            xsd.FindElement("City").Should().NotBeNull();

            var address = xsd.FindElement(Address);
            var city = xsd.FindElement("City");

            address.DataType.Should().BeNullOrEmpty();
            city.DataType.Should().Be("string");

            var child = address.Elements.First(x => x.Name == "City");

            child.DataType.Should().Be("ref");
            child.RefName.Should().Be("City");
            child.MinOccurs.Should().Be("2");
            child.MaxOccurs.Should().Be("23");
        }
    }
}
