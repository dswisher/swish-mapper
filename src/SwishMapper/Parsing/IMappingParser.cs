
using System.Collections.Generic;
using System.Threading.Tasks;

using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public interface IMappingParser
    {
        Task<Mapping> ParseAsync(string path, IEnumerable<DataDocument> sources, IEnumerable<DataDocument> sinks);
    }
}
