
using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;

namespace SwishMapper.Parsing.Project
{
    public class ProjectLexer : AbstractLexer, IDisposable
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>();
        private static readonly HashSet<char> PunctuationStarters = new HashSet<char>();

        static ProjectLexer()
        {
            Keywords.Add("csv");
            Keywords.Add("files");
            Keywords.Add("from");
            Keywords.Add("map");
            Keywords.Add("model");
            Keywords.Add("name");
            Keywords.Add("path");
            Keywords.Add("samples");
            Keywords.Add("to");
            Keywords.Add("xsd");
            Keywords.Add("zip-mask");

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
            : base(reader, path, logger, Keywords, PunctuationStarters)
        {
        }
    }
}
