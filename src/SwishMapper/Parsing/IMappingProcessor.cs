
using System.Collections.Generic;

using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public interface IMappingProcessor
    {
        void Process(Mapping mapping, IEnumerable<DataDocument> sources, IEnumerable<DataDocument> sinks);
    }
}
