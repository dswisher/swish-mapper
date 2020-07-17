
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Tests.TestHelpers;
using SwishMapper.Work;
using Xunit;

namespace SwishMapper.Tests.Work
{
    public class CsvNormalizerTests
    {
        private readonly Mock<ILogger<CsvNormalizer>> logger = new Mock<ILogger<CsvNormalizer>>();

        private readonly CsvNormalizer normalizer;

        public CsvNormalizerTests()
        {
            normalizer = new CsvNormalizer(logger.Object);
        }


        [Fact]
        public async Task CanReadCsvFile()
        {
            // Arrange
            normalizer.Path = FileFinder.FindCsv("monolithic.csv");

            // Act
            var rows = await normalizer.RunAsync();

            // Assert
            rows.Should().HaveCount(4);

            // Element name
            rows[0].ElementName.Should().Be("element1");
            rows[1].ElementName.Should().Be("element1");
            rows[2].ElementName.Should().Be("element1");
            rows[3].ElementName.Should().Be("element2");

            // Attribute name
            rows[0].AttributeName.Should().BeNullOrEmpty();
            rows[1].AttributeName.Should().Be("kind");
            rows[2].AttributeName.Should().BeNullOrEmpty();
            rows[3].AttributeName.Should().BeNullOrEmpty();

            // Child element name
            rows[0].ChildElementName.Should().BeNullOrEmpty();
            rows[1].ChildElementName.Should().BeNullOrEmpty();
            rows[2].ChildElementName.Should().Be("name");
            rows[3].ChildElementName.Should().BeNullOrEmpty();

            // Data type
            rows[0].DataType.Should().BeNullOrEmpty();
            rows[1].DataType.Should().Be("string");
            rows[2].DataType.Should().Be("int");
            rows[3].DataType.Should().BeNullOrEmpty();

            // Max length
            rows[0].MaxLength.Should().BeNullOrEmpty();
            rows[1].MaxLength.Should().Be("13");
            rows[2].MaxLength.Should().Be("2");
            rows[3].MaxLength.Should().BeNullOrEmpty();

            // Required
            rows[0].Required.Should().BeNullOrEmpty();
            rows[1].Required.Should().Be("required");
            rows[2].Required.Should().Be("optional");
            rows[3].Required.Should().BeNullOrEmpty();

            // Min occurences
            rows[0].MinOccurs.Should().BeNullOrEmpty();
            rows[1].MinOccurs.Should().BeNullOrEmpty();
            rows[2].MinOccurs.Should().Be("1");
            rows[3].MinOccurs.Should().BeNullOrEmpty();

            // Max occurences
            rows[0].MaxOccurs.Should().BeNullOrEmpty();
            rows[1].MaxOccurs.Should().BeNullOrEmpty();
            rows[2].MaxOccurs.Should().Be("unbounded");
            rows[3].MaxOccurs.Should().BeNullOrEmpty();

            // Enum values
            rows[0].EnumValues.Should().BeNullOrEmpty();
            rows[1].EnumValues.Should().BeNullOrEmpty();
            rows[2].EnumValues.Should().Be("A|B");
            rows[3].EnumValues.Should().BeNullOrEmpty();

            // Comment
            rows[0].Comment.Should().Be("the first entity");
            rows[1].Comment.Should().Be("the kind of thing");
            rows[2].Comment.Should().BeNullOrEmpty();
            rows[3].Comment.Should().Be("the second entity");
        }
    }
}
