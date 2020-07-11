
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A collection of data models and the mappings between them.
    /// </summary>
    public class DataProject
    {
        private readonly List<DataModel> models = new List<DataModel>();
        private readonly List<DataMapping> maps = new List<DataMapping>();

        public IList<DataModel> Models { get { return models; } }
        public IList<DataMapping> Maps { get { return maps; } }
    }
}
