
using System.Collections.Generic;
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public interface IMapParser
    {
        Task<ExpressiveMapList> ParseAsync(string path, IEnumerable<DataModel> models);
    }
}
