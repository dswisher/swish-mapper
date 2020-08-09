
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A collection of data models and the mappings between them.
    /// </summary>
    public class DataProject
    {
        private readonly List<DataModel> models = new List<DataModel>();
        private readonly List<SimpleDataMapping> simpleMaps = new List<SimpleDataMapping>();
        private readonly List<ExpressiveMapList> expressiveMaps = new List<ExpressiveMapList>();

        public IList<DataModel> Models { get { return models; } }

        // TODO - xyzzy - SimpleMaps should go away
        public IList<SimpleDataMapping> SimpleMaps { get { return simpleMaps; } }
        public IList<ExpressiveMapList> ExpressiveMaps { get { return expressiveMaps; } }
    }
}
