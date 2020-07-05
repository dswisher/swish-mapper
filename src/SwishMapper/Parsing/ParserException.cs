
using System;
using System.Xml.Schema;

namespace SwishMapper.Parsing
{
    /// <summary>
    /// An exception thrown by the various parsers due to a syntax error or the like.
    /// </summary>
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }


        // TODO - figure out how to get filename in here (xsd.SourceUri was null in my testing)
        public ParserException(XmlSchemaObject xsd, string message)
            : base($"line {xsd.LineNumber}: {message}")
        {
            LineNumber = xsd.LineNumber;
            LinePosition = xsd.LinePosition;
        }


        public ParserException(string message, string filename, int lineNumber, int linePosition)
            : base($"{filename} ({lineNumber}:{linePosition}): {message}")
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
            Filename = filename;
        }


        public ParserException(string message, string filename, int lineNumber)
            : base($"{filename} ({lineNumber}): {message}")
        {
            LineNumber = lineNumber;
            Filename = filename;
        }


        public ParserException(string message, LexerToken token)
            : this(message, token.Filename, token.LineNumber, token.LinePosition)
        {
        }


        public int LineNumber { get; private set; }
        public int LinePosition { get; private set; }
        public string Filename { get; private set; }
    }
}
