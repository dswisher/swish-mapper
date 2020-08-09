
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwishMapper.Parsing.Map
{
    public class CompoundIdentifier
    {
        private readonly List<string> parts = new List<string>();

        // TODO - include file/line info so we can provide better error messages

        public CompoundIdentifier()
        {
        }


        public CompoundIdentifier(string s)
        {
            string pieces;
            var bits = s.Split(':');

            if (bits.Length == 1)
            {
                pieces = bits[0];
            }
            else
            {
                Prefix = bits[0];
                pieces = bits[1];
            }

            parts.AddRange(pieces.Split('/'));
        }


        public string Prefix { get; set; }

        public List<string> Parts { get { return parts; } }


        public bool HasPrefix
        {
            get { return !string.IsNullOrEmpty(Prefix); }
        }


        public string XPath
        {
            get { return string.Join("/", parts); }
        }


        public static CompoundIdentifier Parse(MapLexer lexer)
        {
            var ident = new CompoundIdentifier();

            var text = lexer.Consume(TokenKind.Identifier);

            if (lexer.Token.Kind == TokenKind.Colon)
            {
                ident.Prefix = text;

                lexer.Consume(TokenKind.Colon);

                ident.parts.Add(lexer.Consume(TokenKind.Identifier));
            }
            else
            {
                ident.parts.Add(text);
            }

            while (lexer.Token.Kind == TokenKind.Slash)
            {
                lexer.Advance();

                ident.parts.Add(lexer.Consume(TokenKind.Identifier));
            }

            return ident;
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(Prefix))
            {
                builder.Append(Prefix);
                builder.Append(":");
            }

            if (parts.Any())
            {
                builder.Append(string.Join("/", parts));
            }

            return builder.ToString();
        }
    }
}
