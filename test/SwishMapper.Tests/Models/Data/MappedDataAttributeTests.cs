
using System.Linq;

using FluentAssertions;
using SwishMapper.Models.Data;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Models.Data
{
    public class MappedDataAttributeTests
    {
        private readonly DataEntity entity;

        public MappedDataAttributeTests()
        {
            var project = ProjectBuilder.InputOutput();
            var model = project.Models.First(x => x.Id == "input");

            entity = model.FindEntity("Person");
        }


        [Fact]
        public void Equality()
        {
            // Arrange
            var dataAttribute = entity.FindAttribute("Name");

            var mapped1 = new MappedDataAttribute { Attribute = dataAttribute };
            var mapped2 = new MappedDataAttribute { Attribute = dataAttribute };

            // Act and assert
            mapped1.Equals(mapped2).Should().BeTrue();
        }
    }
}
