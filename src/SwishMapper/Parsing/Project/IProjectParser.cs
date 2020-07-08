
using System.Threading.Tasks;

using SwishMapper.Models.Project;

namespace SwishMapper.Parsing.Project
{
    public interface IProjectParser
    {
        Task<ProjectDefinition> ParseAsync(string path);
    }
}
