
using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;

namespace SwishMapper.Parsing.Map
{
    public class MapLexer : AbstractLexer, IDisposable
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>();
        private static readonly HashSet<char> PunctuationStarters = new HashSet<char>();

        static MapLexer()
        {
            Keywords.Add("with");

            PunctuationStarters.Add(';');
            PunctuationStarters.Add('{');
            PunctuationStarters.Add('}');
            PunctuationStarters.Add('-');
        }


        public MapLexer(string path, ILogger<MapLexer> logger)
            : this(new StreamReader(path), path, logger)
        {
        }


        public MapLexer(TextReader reader, string path, ILogger<MapLexer> logger)
            : base(reader, path, logger, Keywords, PunctuationStarters)
        {
        }
    }
}
