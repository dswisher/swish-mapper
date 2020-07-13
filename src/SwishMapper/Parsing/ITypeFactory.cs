
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing
{
    public interface ITypeFactory
    {
        DataType Make(string dataTypeName);
        DataType Make(string dataTypeName, string entityName);
    }
}
