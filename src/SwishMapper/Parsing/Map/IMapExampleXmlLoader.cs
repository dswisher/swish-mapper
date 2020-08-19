
using System.Collections.Generic;

using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public interface IMapExampleXmlLoader
    {
        void LoadExamples(string modelId, string content, IEnumerable<MappedDataAttribute> attributes, string exampleId);
    }
}
