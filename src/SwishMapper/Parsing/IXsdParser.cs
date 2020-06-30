
using System.Threading.Tasks;

using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public interface IXsdParser
    {
        Task<DataDocument> ParseAsync(string path, string docName, string rootElementName, string rootElementNamespace);
    }
}
