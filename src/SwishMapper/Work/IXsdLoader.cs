
using SwishMapper.Models;

namespace SwishMapper.Work
{
    public interface IXsdLoader : IWorker<XsdDocument>
    {
        string Path { get; set; }
        string DebugDumpPath { get; set; }
    }
}
