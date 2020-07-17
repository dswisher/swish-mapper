
using SwishMapper.Models;

namespace SwishMapper.Work
{
    public interface ICsvToXsdTranslator : IWorker<XsdDocument>
    {
        ICsvNormalizer Input { get; set; }
        string DebugDumpPath { get; set; }
    }
}
