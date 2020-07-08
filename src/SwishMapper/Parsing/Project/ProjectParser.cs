
using System.Threading.Tasks;

using SwishMapper.Models.Project;

namespace SwishMapper.Parsing.Project
{
    public class ProjectParser : IProjectParser
    {
        public async Task<ProjectDefinition> ParseAsync(string path)
        {
            await Task.CompletedTask;   // TODO - HACK!

            // TODO - for the moment, just hard-code something

            // One model
            var watchlist = new ProjectModel
            {
                Id = "watchlist",
                Name = "Watchlist Report"
            };

            watchlist.Populators.Add(new ProjectModelPopulator
            {
                Type = ProjectModelPopulatorType.Xsd,
                Path = "SAMPLES/PFA_XSD_22-Dec-07.xsd",
                RootEntity = "PFA"
            });

            // TODO - add samples

            // Two model
            var dmi = new ProjectModel
            {
                Id = "dmi",
                Name = "DMI"

                // TODO - add XSD
                // TODO - add samples
            };

            dmi.Populators.Add(new ProjectModelPopulator
            {
                Type = ProjectModelPopulatorType.Xsd,
                Path = "SAMPLES/DMI_XML.xsd",
                RootEntity = "DMIDocs"
            });

            // Finally, build the overall project definition
            var definition = new ProjectDefinition();

            definition.Models.Add(watchlist);
            definition.Models.Add(dmi);

            return definition;
        }
    }
}
