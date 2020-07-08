
using SwishMapper.Models.Old;

namespace SwishMapper.Parsing.Old
{
    public interface IProjectParser
    {
        ProjectDefinition Parse(string path);
    }
}
