
using System.Threading.Tasks;

using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public interface IXsdParser
    {
        Task<XsdDocument> ParseAsync(string path, string docName, string rootElementName, string rootElementNamespace);
    }
}
