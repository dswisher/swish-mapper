
using FluentAssertions;
using SwishMapper.Sampling;
using Xunit;

namespace SwishMapper.Tests.Sampling
{
    public class SampleAccumulatorTests
    {
        private const string Val = "data1";

        private readonly SampleAccumulator accumulator;

        public SampleAccumulatorTests()
        {
            accumulator = new SampleAccumulator();
        }


        [Theory]
        [InlineData("A")]
        [InlineData("one", "two")]
        [InlineData("uno", "dos", "tres")]
        public void PushingElementsBuildsProperPath(params string[] names)
        {
            // Arrange
            foreach (var name in names)
            {
                accumulator.Push(name);
            }

            // Act
            accumulator.SetValue(Val);

            // Assert
            var key = string.Join("/", names);

            accumulator.Samples.Should().ContainKey(key);
            accumulator.Samples[key].Histogram[Val].Should().Be(1);
        }


        [Fact]
        public void PushingTwoValsAddsToSample()
        {
            // Arrange
            const string key = "thing";

            // Act
            accumulator.Push(key);
            accumulator.SetValue(Val);
            accumulator.Pop();

            accumulator.Push(key);
            accumulator.SetValue(Val);
            accumulator.Pop();

            // Assert
            accumulator.Samples[key].Histogram[Val].Should().Be(2);
        }
    }
}
