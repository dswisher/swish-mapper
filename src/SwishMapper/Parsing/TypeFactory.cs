
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing
{
    public class TypeFactory : ITypeFactory
    {
        private static readonly DataType BoolType = new DataType(PrimitiveType.Boolean);
        private static readonly DataType DateType = new DataType(PrimitiveType.Date);
        private static readonly DataType IntType = new DataType(PrimitiveType.Int);
        private static readonly DataType StringType = new DataType(PrimitiveType.String);


        public DataType Make(string dataTypeName)
        {
            return Make(dataTypeName, null);
        }


        public DataType Make(string dataTypeName, string entityName)
        {
            switch (dataTypeName.ToUpper())
            {
                case "BOOLEAN":
                    return BoolType;

                case "DATETIME":
                    return DateType;

                case "INT16":
                case "INT32":
                case "INT64":
                case "INTEGER":
                    return IntType;

                case "NMTOKEN":
                    // TODO - xyzzy - what about the enum values?
                    return new DataType(PrimitiveType.Enum);

                case "REF":
                    return new DataType(entityName);

                case "STRING":
                    return StringType;

                default:
                    throw new TypeException($"Unable to construct a type from dataTypeName '{dataTypeName}' and entityName '{entityName}'.");
            }
        }
    }
}
