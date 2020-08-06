
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class ExpressiveMapList
    {
        private readonly List<ExpressiveMapping> maps = new List<ExpressiveMapping>();

        public IList<ExpressiveMapping> Maps { get { return maps; } }
    }
}
