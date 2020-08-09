
namespace SwishMapper.Parsing
{
    public static class LexerExtensions
    {
        public static void VerifyToken(this AbstractLexer lexer, params TokenKind[] kinds)
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


        public static void VerifyToken(this AbstractLexer lexer, TokenKind kind, string text = null)
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


        public static void Advance(this AbstractLexer lexer)
        {
            lexer.LexToken();
        }


        public static string Consume(this AbstractLexer lexer, TokenKind kind, string text = null)
        {
            lexer.VerifyToken(kind, text);

            var result = lexer.Token.Text;

            lexer.Advance();

            return result;
        }


        public static string Consume(this AbstractLexer lexer, params TokenKind[] kinds)
        {
            lexer.VerifyToken(kinds);

            var result = lexer.Token.Text;

            lexer.Advance();

            return result;
        }
    }
}
