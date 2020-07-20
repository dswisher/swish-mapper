
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwishMapper.Models.Data;
using SwishMapper.Work;
using Xunit;

namespace SwishMapper.Tests.Work
{
    public class EmptyEntityCleanerTests
    {
        private const string Person = "Person";
        private const string Name = "Name";
        private const int MaxLen = 12;

        private readonly Mock<ILogger<EmptyEntityCleaner>> logger = new Mock<ILogger<EmptyEntityCleaner>>();
        private readonly Mock<IModelProducer> input = new Mock<IModelProducer>();

        private readonly DataModelSource source;

        private readonly EmptyEntityCleaner cleaner;

        public EmptyEntityCleanerTests()
        {
            source = new DataModelSource
            {
                ShortName = "test",
                Path = "/dev/null"
            };

            cleaner = new EmptyEntityCleaner(logger.Object);

            cleaner.Input = input.Object;
        }


        [Fact]
        public async Task EmptyModelIsHandled()
        {
            // Arrange
            var inputModel = new DataModel();

            input.Setup(x => x.RunAsync()).ReturnsAsync(inputModel);

            // Act
            var outputModel = await cleaner.RunAsync();

            // Assert
            outputModel.Should().NotBeNull();
            outputModel.Entities.Should().BeEmpty();
        }


        [Fact]
        public async Task EntityWithAttributeComesThroughUnscathed()
        {
            // Arrange
            var inputModel = new DataModel();
            var entity = inputModel.FindOrCreateEntity(Person, source);
            var att = entity.FindOrCreateAttribute(Name, source);
            att.DataType = new DataType(PrimitiveType.String, 10);

            input.Setup(x => x.RunAsync()).ReturnsAsync(inputModel);

            // Act
            var outputModel = await cleaner.RunAsync();

            // Assert
            outputModel.FindEntity(Person).Should().NotBeNull();
        }


        [Fact]
        public async Task EmptyRefIsFixedUp()
        {
            // Arrange
            var inputModel = new DataModel();
            var person = inputModel.FindOrCreateEntity(Person, source);
            var att = person.FindOrCreateAttribute(Name, source);
            att.DataType = new DataType(Name);

            var name = inputModel.FindOrCreateEntity(Name, source);
            name.DataType = new DataType(PrimitiveType.String, MaxLen);

            input.Setup(x => x.RunAsync()).ReturnsAsync(inputModel);

            // Act
            var outputModel = await cleaner.RunAsync();

            // Assert
            outputModel.FindEntity(Person).Should().NotBeNull();
            outputModel.FindEntity(Name).Should().BeNull();

            person = outputModel.FindEntity(Person);
            att = person.FindAttribute(Name);

            att.DataType.Type.Should().Be(PrimitiveType.String);
            att.DataType.MaxLength.Should().Be(MaxLen);
        }
    }
}
