
using System.Collections.Generic;

namespace SwishMapper.Parsing.Map
{
    public class MapParserExamplesModel
    {
        private readonly List<string> directories = new List<string>();

        public MapParserExamplesModel(string modelId)
        {
            ModelId = modelId;
        }


        public string ModelId { get; private set; }
        public IList<string> Directories { get { return directories; } }

        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }
}
