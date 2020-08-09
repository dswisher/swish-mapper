
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class ExpressiveMapList
    {
        private readonly List<ExpressiveMapping> maps = new List<ExpressiveMapping>();

        public ExpressiveMapList(string filename)
        {
            FileName = filename;
        }

        public string FileName { get; private set; }
        public string Name { get; set; }
        public IList<ExpressiveMapping> Maps { get { return maps; } }
    }
}
