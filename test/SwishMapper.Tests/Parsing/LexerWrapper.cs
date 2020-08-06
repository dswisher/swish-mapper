
using System;
using System.IO;

using Microsoft.Extensions.Logging;
using Moq;

namespace SwishMapper.Tests.Parsing
{
    public abstract class LexerWrapper<TLexer> : IDisposable
        where TLexer : IDisposable
    {
        public const string Filename = "mem-stream.txt";

        private readonly Mock<ILogger<TLexer>> logger = new Mock<ILogger<TLexer>>();
        private readonly MemoryStream memoryStream;


        public LexerWrapper(string input)
        {
            memoryStream = new MemoryStream();

            using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
            {
                writer.Write(input);
                writer.Flush();
            }

            memoryStream.Position = 0;

            var reader = new StreamReader(memoryStream, leaveOpen: true);
            Lexer = MakeLexer(reader, Filename, logger.Object);
        }


        public TLexer Lexer { get; private set; }


        public void Dispose()
        {
            Lexer.Dispose();
        }


        protected abstract TLexer MakeLexer(StreamReader reader, string filename, ILogger<TLexer> logger);
    }
}
