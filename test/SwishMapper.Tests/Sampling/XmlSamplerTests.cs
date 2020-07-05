
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Sampling;
using Xunit;

namespace SwishMapper.Tests.Sampling
{
    public class XmlSamplerTests
    {
        private readonly Mock<ILogger<XmlSampler>> logger = new Mock<ILogger<XmlSampler>>();
        private readonly TestAccumulator accumulator = new TestAccumulator();

        private readonly XmlSampler sampler;


        public XmlSamplerTests()
        {
            sampler = new XmlSampler(logger.Object);
        }


        [Theory]
        [InlineData("<elem></elem>", new[] { "elem" })]
        // TODO - xyzzy - implement this!
        // [InlineData("<parent><child></child></parent>", new[] { "parent", "child" })]
        public async Task ElementsAreSampled(string content, IEnumerable<string> expectedElements)
        {
            // Arrange
            using (var stream = Content(content))
            {
                // Act
                await sampler.SampleAsync(stream, accumulator);

                // Assert
                foreach (var elem in expectedElements)
                {
                    accumulator.Saw(elem).Should().BeTrue();
                }
            }
        }


        private SampleStream Content(string content)
        {
            var memory = new MemoryStream();

            using (var writer = new StreamWriter(memory, leaveOpen: true))
            {
                writer.Write(content);
            }

            memory.Position = 0;

            return new SampleStream("foo.xml", memory);
        }


        private class TestAccumulator : ISampleAccumulator
        {
            private readonly HashSet<string> pushed = new HashSet<string>();

            public void Push(string name)
            {
                pushed.Add(name);
            }

            public void Pop()
            {
            }

            public bool Saw(string name)
            {
                return pushed.Contains(name);
            }
        }
    }
}
