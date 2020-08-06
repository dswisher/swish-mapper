
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwishMapper.Parsing.Map
{
    public class CompoundIdentifier
    {
        private readonly List<string> parts = new List<string>();

        public string Prefix { get; private set; }

        public IEnumerable<string> Parts { get { return parts; } }


        public static CompoundIdentifier Parse(MapLexer lexer)
        {
            var ident = new CompoundIdentifier();

            var text = AbstractParser.Consume(lexer, TokenKind.Identifier);

            if (lexer.Token.Kind == TokenKind.Colon)
            {
                ident.Prefix = text;

                AbstractParser.Consume(lexer, TokenKind.Colon);

                ident.parts.Add(AbstractParser.Consume(lexer, TokenKind.Identifier));
            }
            else
            {
                ident.parts.Add(text);
            }

            while (lexer.Token.Kind == TokenKind.Slash)
            {
                AbstractParser.Advance(lexer);

                ident.parts.Add(AbstractParser.Consume(lexer, TokenKind.Identifier));
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
