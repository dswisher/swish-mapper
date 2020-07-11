
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// One set of mappings between two data models.
    /// </summary>
    public class DataMapping
    {
        private readonly List<DataAttributeMap> maps = new List<DataAttributeMap>();

        public DataModel SourceModel { get; set; }
        public DataModel SinkModel { get; set; }

        public IList<DataAttributeMap> Maps { get { return maps; } }
    }
}
