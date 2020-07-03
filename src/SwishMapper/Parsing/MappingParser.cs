
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public class MappingParser : IMappingParser
    {
        private readonly ILexerFactory lexerFactory;
        private readonly ILogger logger;

        public MappingParser(ILexerFactory lexerFactory,
                             ILogger<MappingParser> logger)
        {
            this.lexerFactory = lexerFactory;
            this.logger = logger;
        }


        public async Task<Mapping> ParseAsync(string path, IEnumerable<DataDocument> sources, IEnumerable<DataDocument> sinks)
        {
            var mapFileInfo = new FileInfo(path);

            var mapping = new Mapping();

            using (var lexer = lexerFactory.CreateMappingLexer(mapFileInfo.FullName))
            {
                // Move to the first token
                // lexer.Advance();

                // while (lexer.Token.Kind != TokenKind.EOF)
                // {
                //     // ParseDirective(...);
                //     lexer.Advance();
                // }

                // Keep parsing tokens until we hit EOF
                // TODO

                // TODO - switch to using the lexer to parse!
            }

            using (var reader = new StreamReader(mapFileInfo.FullName))
            {
                string line;
                int lineNumber = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber += 1;

                    // Skip comments and whitespace
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Split into words
                    var bits = line.Split();

                    // Take the proper action
                    switch (bits[0])
                    {
                        case "sink":
                            mapping.Sink = FindItemByName(sinks, bits[1], mapFileInfo, lineNumber);
                            break;

                        case "source":
                            mapping.AddSource(FindItemByName(sources, bits[1], mapFileInfo, lineNumber));
                            break;

                        case "mappings":
                            break;

                        default:
                            // TODO - someday, keep track of line position (column) so it can be reported
                            throw new ParserException($"Unknown directive: '{bits[0]}'.", mapFileInfo.Name, lineNumber);
                    }
                }
            }

            return mapping;
        }


        private DataDocument FindItemByName(IEnumerable<DataDocument> docs, string name, FileInfo mapFileInfo, int lineNumber)
        {
            var item = docs.FirstOrDefault(x => x.Name == name);

            if (item == null)
            {
                throw new ParserException($"Could not find sink '{name}'.", mapFileInfo.Name, lineNumber);
            }

            return item;
        }
    }
}
