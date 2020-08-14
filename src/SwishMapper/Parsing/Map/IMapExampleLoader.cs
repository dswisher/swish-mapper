
using System.Threading.Tasks;

namespace SwishMapper.Parsing.Map
{
    public interface IMapExampleLoader
    {
        Task LoadAsync(MapParserContext context);
    }
}
