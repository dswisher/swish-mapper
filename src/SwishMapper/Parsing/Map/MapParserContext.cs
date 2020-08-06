
using System.Collections.Generic;

using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapParserContext
    {
        public ExpressiveMapList MapList { get; set; }
        public IEnumerable<DataModel> Models { get; set; }
    }
}
