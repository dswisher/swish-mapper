
using System.Threading.Tasks;

using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public interface IMappingParser
    {
        Task<Mapping> ParseAsync(string path);
    }
}
