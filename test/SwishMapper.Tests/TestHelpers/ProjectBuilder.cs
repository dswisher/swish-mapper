
using SwishMapper.Models.Data;

namespace SwishMapper.Tests.TestHelpers
{
    public class ProjectBuilder
    {
        private readonly DataProject project = new DataProject();
        private readonly DataModelSource source = new DataModelSource
        {
            ShortName = "ProjectBuilder",
            Path = "ProjectBuilder"
        };

        private DataModel currentModel;
        private DataEntity currentEntity;
        private DataAttribute currentAttribute;


        public static DataProject Predefined(string name)
        {
            switch (name)
            {
                case "input-output":
                    return InputOutput();

                default:
                    throw new System.NotImplementedException($"Predefined project '{name}' has not been set up in ProjectBuilder.");
            }
        }


        public static DataProject InputOutput()
        {
            var builder = new ProjectBuilder();

            builder.Model("input")
                .Entity("Person")
                    .Attribute("Name")
                    .Attribute("FirstName")
                    .Attribute("MiddleName")
                    .Attribute("LastName")
                    .Attribute("BirthPlace")
                        .References("City")
                .Entity("City")
                    .Attribute("Name");

            builder.Model("output")
                .Entity("People")
                    .Attribute("FullName")
                    .Attribute("PlaceOfBirth")
                        .References("Place")
                .Entity("Place")
                    .Attribute("Name");

            return builder.project;
        }


        public ProjectBuilder Model(string id)
        {
            currentModel = new DataModel
            {
                Id = id,
                Name = id
            };

            project.Models.Add(currentModel);

            return this;
        }


        public ProjectBuilder Entity(string name)
        {
            currentEntity = currentModel.FindOrCreateEntity(name, source);

            return this;
        }


        public ProjectBuilder Attribute(string name)
        {
            currentAttribute = currentEntity.FindOrCreateAttribute(name, source);

            // Default the datatype to string
            currentAttribute.DataType = new DataType(PrimitiveType.String, 255);

            return this;
        }


        public ProjectBuilder References(string name)
        {
            currentAttribute.DataType = new DataType(name);

            return this;
        }
    }
}
