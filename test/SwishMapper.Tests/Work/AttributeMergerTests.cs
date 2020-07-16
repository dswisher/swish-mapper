
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models.Data;
using SwishMapper.Work;
using Xunit;

namespace SwishMapper.Tests.Work
{
    public class AttributeMergerTests
    {
        private readonly Mock<ILogger<AttributeMerger>> logger = new Mock<ILogger<AttributeMerger>>();

        private readonly AttributeMerger merger;

        private readonly DataModelSource sourceSource;
        private readonly DataModelSource targetSource;
        private readonly DataModel sourceModel;
        private readonly DataModel targetModel;
        private readonly DataEntity sourceEntity;
        private readonly DataEntity targetEntity;
        private readonly DataAttribute sourceAttribute;
        private readonly DataAttribute targetAttribute;

        public AttributeMergerTests()
        {
            sourceSource = new DataModelSource();
            targetSource = new DataModelSource();

            sourceModel = new DataModel();
            targetModel = new DataModel();

            sourceEntity = sourceModel.FindOrCreateEntity("entity", sourceSource);
            targetEntity = targetModel.FindOrCreateEntity("entity", targetSource);

            sourceAttribute = sourceEntity.FindOrCreateAttribute("att", sourceSource);
            targetAttribute = targetEntity.FindOrCreateAttribute("att", targetSource);

            merger = new AttributeMerger(logger.Object);
        }


        [Theory]
        [InlineData("string", "int")]
        [InlineData("boolean", "string")]
        [InlineData("date", "int")]
        [InlineData("string(12)", "string(8)")]
        [InlineData("string", "string(8)")]
        [InlineData("ref(this)", "ref(that)")]
        public void ConflictingDataTypesAreMismatches(string sourceType, string targetType)
        {
            // Arrange
            sourceAttribute.DataType = DataType.FromString(sourceType);
            targetAttribute.DataType = DataType.FromString(targetType);

            // Act
            var mismatchCount = merger.Merge(targetAttribute, sourceAttribute);

            // Assert
            targetAttribute.DataType.ToString().Should().Be(targetType);
            mismatchCount.Should().Be(1);
        }


        [Theory]
        [InlineData("boolean")]
        [InlineData("date")]
        [InlineData("int")]
        [InlineData("string")]
        [InlineData("string(12)")]
        [InlineData("ref(thing)")]
        public void SameDataTypesAreNotMismatches(string type)
        {
            // Arrange
            sourceAttribute.DataType = DataType.FromString(type);
            targetAttribute.DataType = DataType.FromString(type);

            // Act
            var mismatchCount = merger.Merge(targetAttribute, sourceAttribute);

            // Assert
            mismatchCount.Should().Be(0);
        }


        [Theory]
        [InlineData("string(12)", "string")]
        public void StringWithLengthDoesNotConflictWithString(string sourceType, string targetType)
        {
            // Arrange
            sourceAttribute.DataType = DataType.FromString(sourceType);
            targetAttribute.DataType = DataType.FromString(targetType);

            // Act
            var mismatchCount = merger.Merge(targetAttribute, sourceAttribute);

            // Assert
            targetAttribute.DataType.ToString().Should().Be(sourceType);
            mismatchCount.Should().Be(0);
        }
    }
}
