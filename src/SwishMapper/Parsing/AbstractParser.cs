
namespace SwishMapper.Parsing
{
    public abstract class AbstractParser
    {
        public static void VerifyToken(AbstractLexer lexer, params TokenKind[] kinds)
        {
            foreach (var kind in kinds)
            {
                if (lexer.Token.Kind == kind)
                {
                    return;
                }
            }

            var list = string.Join(", ", kinds);

            throw new ParserException($"Expecting token to be one of ({list}), but found {lexer.Token.Kind}.", lexer.Token);
        }


        public static void VerifyToken(AbstractLexer lexer, TokenKind kind, string text = null)
        {
            if (lexer.Token.Kind != kind)
            {
                throw new ParserException($"Expecting token kind {kind}, but found {lexer.Token.Kind}: {lexer.Token.Text}.", lexer.Token);
            }

            if ((text != null) && (lexer.Token.Text != text))
            {
                throw new ParserException($"Expecting {kind} token to have text '{text}', but found '{lexer.Token.Text}'.", lexer.Token);
            }
        }


        public static void Advance(AbstractLexer lexer)
        {
            lexer.LexToken();
        }


        public static string Consume(AbstractLexer lexer, TokenKind kind, string text = null)
        {
            VerifyToken(lexer, kind, text);

            var result = lexer.Token.Text;

            Advance(lexer);

            return result;
        }
    }
}
