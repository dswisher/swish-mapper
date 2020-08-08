
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
    }
}
