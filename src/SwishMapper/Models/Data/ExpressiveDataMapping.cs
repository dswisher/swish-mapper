
namespace SwishMapper.Models.Data
{
    public class ExpressiveDataMapping
    {
        // TODO - xyzzy - replace with a most expressive attribute mapping
        // private readonly List<DataAttributeMap> maps = new List<DataAttributeMap>();

        public DataModel SourceModel { get; set; }
        public DataModel SinkModel { get; set; }

        // public IList<DataAttributeMap> Maps { get { return maps; } }
    }
}
