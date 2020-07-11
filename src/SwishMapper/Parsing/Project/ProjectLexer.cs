
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Extensions.Logging;

namespace SwishMapper.Parsing.Project
{
    public class ProjectLexer : IDisposable
    {
        private static readonly HashSet<string> Keywords;
        private static readonly HashSet<char> PunctuationStarters;

        private readonly ILogger logger;
        private readonly TextReader reader;

        private readonly string filename;

        private readonly StringBuilder builder = new StringBuilder();

        private bool atEOF;
        private int lineNumber;
        private int linePos;


        static ProjectLexer()
        {
            Keywords = new HashSet<string>();

            Keywords.Add("csv");
            Keywords.Add("from");
            Keywords.Add("map");
            Keywords.Add("model");
            Keywords.Add("name");
            Keywords.Add("path");
            Keywords.Add("root");
            Keywords.Add("to");
            Keywords.Add("xsd");

            PunctuationStarters = new HashSet<char>();

            PunctuationStarters.Add(';');
            PunctuationStarters.Add('{');
            PunctuationStarters.Add('}');
            PunctuationStarters.Add('-');
        }


        public ProjectLexer(string path, ILogger<ProjectLexer> logger)
            : this(new StreamReader(path), path, logger)
        {
        }


        public ProjectLexer(TextReader reader, string path, ILogger<ProjectLexer> logger)
        {
            this.logger = logger;
            this.reader = reader;

            FilePath = path;

            filename = Path.GetFileName(path);

            lineNumber = 1;
            linePos = 1;
        }


        public string FilePath { get; private set; }
        public LexerToken Token { get; private set; }

        private char Current => (char)reader.Peek();


        public void LexToken()
        {
            // Stop when we hit end of file
            if (atEOF)
            {
                return;
            }

            // Skip any leading whitespace
            SkipWhiteSpace();

            // Do the actual lexing
            if (IsEOF())
            {
                CreateToken(TokenKind.EOF);
                atEOF = true;
            }
            else if (Current == '"')
            {
                ScanString();
            }
            else if (char.IsLetter(Current))
            {
                ScanIdentifierOrKeyword();
            }
            else if (PunctuationStarters.Contains(Current))
            {
                ScanPunctuation();
            }
            else
            {
                throw new ParserException($"Unexpected character '{Current}'.", filename, lineNumber, linePos);
            }
        }


        public void Dispose()
        {
            try
            {
                reader.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error disposing MappingLexer, filename: {Name}.", filename);
            }
        }


        private void Advance()
        {
            if (reader.Read() == '\n')
            {
                lineNumber += 1;
                linePos = 1;
            }
            else
            {
                linePos += 1;
            }
        }


        private void Consume()
        {
            builder.Append(Current);
            Advance();
        }


        private void CreateToken(TokenKind kind)
        {
            Token = new LexerToken
            {
                Kind = kind,
                Text = builder.ToString(),
                LineNumber = lineNumber,
                LinePosition = linePos,
                Filename = filename
            };

            builder.Clear();
        }


        private bool IsEOF()
        {
            return atEOF || (reader.Peek() == -1);
        }


        private void ScanIdentifierOrKeyword()
        {
            while (char.IsLetter(Current) || char.IsDigit(Current) || (Current == '_') || (Current == '-'))
            {
                Consume();
            }

            CreateToken(Keywords.Contains(builder.ToString()) ? TokenKind.Keyword : TokenKind.Identifier);
        }


        private void ScanString()
        {
            Advance();
            while (!IsEOF() && (Current != '"'))
            {
                Consume();
            }

            Advance();

            CreateToken(TokenKind.String);
        }


        private void ScanPunctuation()
        {
            switch (Current)
            {
                case '{':
                    Consume();
                    CreateToken(TokenKind.LeftCurly);
                    break;

                case '}':
                    Consume();
                    CreateToken(TokenKind.RightCurly);
                    break;

                case ';':
                    Consume();
                    CreateToken(TokenKind.Semicolon);
                    break;

                case '-':
                    ScanArrow();
                    break;

                default:
                    throw new ParserException($"Unexpected punctuation character '{Current}'.", filename, lineNumber, linePos);
            }
        }


        private void ScanArrow()
        {
            // TODO - backtrack?
            Consume();

            if (Current == '>')
            {
                Consume();
                CreateToken(TokenKind.Arrow);
            }
            else
            {
                throw new ParserException($"Unexpected arrow character '{Current}'.", filename, lineNumber, linePos);
            }
        }

        private void SkipWhiteSpace()
        {
            while (!IsEOF())
            {
                // Skip true whitespace
                while (char.IsWhiteSpace(Current))
                {
                    Advance();
                }

                // Do we have a comment to skip?
                if (Current != '#')
                {
                    return;
                }

                while (!IsEOF() && (Current != '\n'))
                {
                    Advance();
                }
            }
        }
    }
}
