
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing
{
    public class TypeFactory : ITypeFactory
    {
        private static readonly DataType BoolType = new DataType(PrimitiveType.Boolean);
        private static readonly DataType DateType = new DataType(PrimitiveType.Date);
        private static readonly DataType IntType = new DataType(PrimitiveType.Int);
        private static readonly DataType DecimalType = new DataType(PrimitiveType.Decimal);
        private static readonly DataType StringType = new DataType(PrimitiveType.String);


        public DataType Make(string dataTypeName, int? maxLength)
        {
            return Make(dataTypeName, null, maxLength);
        }


        public DataType Make(string dataTypeName, string entityName, int? maxLength)
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
                case "SBYTE":
                    // TODO - should these be separate types?
                    return IntType;

                case "DECIMAL":
                    return DecimalType;

                case "NMTOKEN":
                    return new DataType(PrimitiveType.Enum);

                case "REF":
                    return new DataType(entityName);

                case "STRING":
                    if (maxLength.HasValue)
                    {
                        return new DataType(PrimitiveType.String, maxLength);
                    }
                    else
                    {
                        return StringType;
                    }

                default:
                    throw new TypeException($"Unable to construct a type from dataTypeName '{dataTypeName}' and entityName '{entityName}'.");
            }
        }
    }
}
