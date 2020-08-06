
using SwishMapper.Models.Data;

namespace SwishMapper.Tests.TestHelpers
{
    public class ProjectBuilder
    {
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
            var input = new DataModel
            {
                Id = "input",
                Name = "Input"
            };

            var output = new DataModel
            {
                Id = "output",
                Name = "Output"
            };

            // TODO - xyzzy - add entities/attributes to the models

            var project = new DataProject();
            project.Models.Add(input);
            project.Models.Add(output);

            return project;
        }
    }
}
