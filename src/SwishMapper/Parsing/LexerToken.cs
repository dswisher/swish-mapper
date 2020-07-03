
namespace SwishMapper.Parsing
{
    public enum TokenKind
    {
        EOF,
        Identifier,
        Keyword,
        Arrow,      // ->
        LeftCurly,
        RightCurly
    }


    public class LexerToken
    {
        public TokenKind Kind { get; set; }
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
        public string Filename { get; set; }
        public string Text { get; set; }
    }
}
