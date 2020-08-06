
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public interface IMapParser
    {
        Task<ExpressiveDataMapping> ParseAsync(string path);
    }
}
