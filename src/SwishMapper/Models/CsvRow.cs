
namespace SwishMapper.Models
{
    // TODO - rename to something that reflects the fact it is XML-ish
    public class CsvRow
    {
        public string ElementName { get; set; }
        public string AttributeName { get; set; }
        public string ChildElementName { get; set; }

        public string DataType { get; set; }
        public string MaxLength { get; set; }
        public string Required { get; set; }
        public string MinOccurs { get; set; }
        public string MaxOccurs { get; set; }
        public string EnumValues { get; set; }
        public string Comment { get; set; }
    }
}
