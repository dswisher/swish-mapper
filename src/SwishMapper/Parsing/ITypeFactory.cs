
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing
{
    public interface ITypeFactory
    {
        DataType Make(string dataTypeName, int? maxLength = null);
        DataType Make(string dataTypeName, string entityName, int? maxLength = null);
    }
}
