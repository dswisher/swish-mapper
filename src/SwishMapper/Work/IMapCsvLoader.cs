
namespace SwishMapper.Work
{
    public interface IMapCsvLoader : IMapLoader
    {
        string FromModelId { set; }
        string ToModelId { set; }
    }
}
