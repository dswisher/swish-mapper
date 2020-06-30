
using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public interface IProjectParser
    {
        ProjectDefinition Parse(string path);
    }
}
