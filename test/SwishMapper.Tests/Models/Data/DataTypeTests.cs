
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
    }
}
