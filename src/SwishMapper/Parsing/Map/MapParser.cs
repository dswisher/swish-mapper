
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapParser : IMapParser
    {
        public async Task<ExpressiveDataMapping> ParseAsync(string path)
        {
            // TODO - xyzzy - implement me!
            await Task.CompletedTask;

            return new ExpressiveDataMapping();
        }
    }
}
