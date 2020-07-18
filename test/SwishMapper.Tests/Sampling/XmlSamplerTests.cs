
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models;
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
        [InlineData("<parent><child></child></parent>", new[] { "parent", "child" })]
        [InlineData("<parent><brother></brother><sister></sister></parent>", new[] { "parent", "brother", "sister" })]
        [InlineData("<parent><child><grandchild></grandchild></child><sibling></sibling></parent>",
                new[] { "parent", "child", "grandchild", "sibling" })]
        [InlineData("<parent><empty/></parent>", new[] { "parent", "empty" })]
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
                    accumulator.Saw(elem).Should().BeTrue($"{elem} was included in the XML");
                }
            }
        }


        [Theory]
        [InlineData("<elem>data</elem>", "elem", "data")]
        [InlineData("<parent><child1>data1</child1><child2>data2</child2></parent>", "child2", "data2")]
        public async Task ElementsWithContentAreSampled(string content, string name, string val)
        {
            // Arrange
            using (var stream = Content(content))
            {
                // Act
                await sampler.SampleAsync(stream, accumulator);

                // Assert
                accumulator.Saw(name).Should().BeTrue();

                accumulator.GetValue(name).Should().Be(val);
            }
        }


        [Theory]
        [InlineData("<elem id=\"1234\"></elem>", "id", "1234")]
        [InlineData("<parent><elem id=\"1234\"></elem></parent>", "id", "1234")]
        public async Task AttributesAreSampled(string content, string name, string val)
        {
            // Arrange
            using (var stream = Content(content))
            {
                // Act
                await sampler.SampleAsync(stream, accumulator);

                // Assert
                accumulator.Saw(name).Should().BeTrue();

                accumulator.GetValue(name).Should().Be(val);
            }
        }


        [Theory]
        [InlineData("<stuff xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"></stuff>", "xmlns:xsi")]
        [InlineData("<stuff xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"></stuff>", "xmlns:xsd")]
        public async Task RootNamespacesAreIgnored(string content, string name)
        {
            // Arrange
            using (var stream = Content(content))
            {
                // Act
                await sampler.SampleAsync(stream, accumulator);

                // Assert
                accumulator.Saw(name).Should().BeFalse();
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
            private readonly HashSet<string> seen = new HashSet<string>();
            private readonly Dictionary<string, string> values = new Dictionary<string, string>();
            private readonly Stack<string> current = new Stack<string>();

            public IDictionary<string, Sample> Samples => throw new System.NotImplementedException();
            public IList<string> Filenames => throw new System.NotImplementedException();

            public void AddFile(string filename)
            {
            }

            public void Push(string name, bool isAttribute = false)
            {
                seen.Add(name);

                current.Push(name);
            }

            public void Pop()
            {
                current.Pop();
            }

            public bool Saw(string name)
            {
                return seen.Contains(name);
            }

            public void SetValue(string val)
            {
                values[current.Peek()] = val;
            }

            public string GetValue(string name)
            {
                return values.ContainsKey(name) ? values[name] : null;
            }

            public SampleJson AsJson()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
