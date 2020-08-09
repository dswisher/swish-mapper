
using System;
using System.Linq;

using FluentAssertions;
using SwishMapper.Models.Data;
using SwishMapper.Parsing;
using SwishMapper.Parsing.Map;
using SwishMapper.Tests.TestHelpers;
using Xunit;

namespace SwishMapper.Tests.Parsing.Map
{
    public class MapParserContextTests
    {
        [Theory]
        [InlineData("input-output", "input:Person/Name", "input", "Person", "Name")]
        [InlineData("input-output", "input:Person/BirthPlace/Name", "input", "City", "Name")]
        public void CanResolveIdentifier(string projectName, string content, string modelId, string entityName, string attributeName)
        {
            // Arrange
            var context = MakeContext(projectName);
            var ident = new CompoundIdentifier(content);

            var expected = FindAttribute(context, modelId, entityName, attributeName);

            // Act
            var resolved = context.Resolve(ident);

            // Assert
            resolved.Attribute.Should().Be(expected);
            resolved.XPath.Should().Be(ident.XPath);
        }


        [Theory]
        [InlineData("input-output", "Person/Name", "must have a prefix")]
        [InlineData("input-output", "foo:Person/Name", "not find a model")]
        [InlineData("input-output", "input:Entity/Name", "not find*entity")]
        [InlineData("input-output", "input:Person/Attribute", "not find attribute")]
        [InlineData("input-output", "input:Person/Nope/Name", "not find attribute")]
        [InlineData("input-output", "input:Person/Name/Name", "not*reference")]
        public void UnresolvableIdentifierThrows(string modelName, string content, string message)
        {
            // Arrange
            var context = MakeContext(modelName);
            var ident = new CompoundIdentifier(content);

            Action act = () => context.Resolve(ident);

            // Act and assert
            act.Should().Throw<ParserException>()
                .WithMessage($"*{message}*");
        }


        [Fact]
        public void PrefixesAreAvailableWithinTheirScope()
        {
            // Arrange
            var context = MakeContext("input-output");
            var alias = new CompoundIdentifier("input:City");
            var ident = new CompoundIdentifier("scope:Name");

            var expected = context.Models.First(x => x.Id == "input").FindEntity("City").FindAttribute("Name");

            // Act
            context.Push("scope", alias);
            var resolved = context.Resolve(ident);
            context.Pop();

            // Assert
            resolved.Attribute.Should().Be(expected);

            resolved.XPath.Should().Be("City/Name");
        }


        private MapParserContext MakeContext(string modelName)
        {
            var project = ProjectBuilder.Predefined(modelName);

            var context = new MapParserContext("foo.txt", project.Models);

            foreach (var model in project.Models)
            {
                context.AddModelAlias(model.Id, model);
            }

            return context;
        }


        private DataAttribute FindAttribute(MapParserContext context, string modelId, string entityName, string attributeName)
        {
            var model = context.Models.First(x => x.Id == modelId);
            var entity = model.Entities.First(x => x.Name == entityName);
            return entity.Attributes.First(x => x.Name == attributeName);
        }
    }
}
