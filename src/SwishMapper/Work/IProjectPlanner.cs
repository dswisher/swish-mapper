
using SwishMapper.Models.Project;

namespace SwishMapper.Work
{
    public interface IProjectPlanner
    {
        DataProjectAssembler CreateWork(ProjectDefinition project);
    }
}
