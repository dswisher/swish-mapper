
using System;
using System.Xml.Schema;

namespace SwishMapper.Parsing
{
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }


        // TODO - figure out how to get filename in here
        public ParserException(XmlSchemaObject xsd, string message)
            : base($"line {xsd.LineNumber}: {message}")
        {
        }
    }
}
