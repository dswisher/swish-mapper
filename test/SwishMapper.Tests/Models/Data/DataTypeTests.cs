
using FluentAssertions;
using SwishMapper.Models.Data;
using Xunit;

namespace SwishMapper.Tests.Models.Data
{
    public class DataTypeTests
    {
        [Theory]
        [InlineData(PrimitiveType.Int)]
        public void NonRefTypesAreEqual(PrimitiveType type)
        {
            // Arrange
            var t1 = new DataType(type);
            var t2 = new DataType(type);

            // Act and Assert
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
            (t1 == t2).Should().BeTrue();
            (t2 == t1).Should().BeTrue();
        }


        [Theory]
        [InlineData("boolean", PrimitiveType.Boolean)]
        [InlineData("date", PrimitiveType.Date)]
        [InlineData("enum", PrimitiveType.Enum)]
        [InlineData("int", PrimitiveType.Int)]
        [InlineData("string", PrimitiveType.String)]
        [InlineData("string(5)", PrimitiveType.String, 5)]
        [InlineData("ref(thing)", PrimitiveType.Ref, null, "thing")]
        public void CanMakeStuffFromStrings(string type, PrimitiveType expectedPrimitiveType, int? maxLength = null, string refName = null)
        {
            // Act
            var dataType = DataType.FromString(type);

            // Assert
            dataType.Type.Should().Be(expectedPrimitiveType);
            dataType.MaxLength.Should().Be(maxLength);
            dataType.RefName.Should().Be(refName);
        }
    }
}
