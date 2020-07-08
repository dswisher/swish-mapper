
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;

namespace SwishMapper.Parsing
{
    public class MappingProcessor : IMappingProcessor
    {
        private readonly ILogger logger;

        public MappingProcessor(ILogger<MappingProcessor> logger)
        {
            this.logger = logger;
        }


        public void Process(Mapping mapping, IEnumerable<XsdDocument> allSources, IEnumerable<XsdDocument> allSinks)
        {
            // Find the sink
            var sink = allSinks.FirstOrDefault(x => x.Name == mapping.SinkName);

            if (sink == null)
            {
                // TODO - replace with a "mapping" exception or some such
                throw new ParserException($"Could not locate sink '{mapping.SinkName}'.");
            }

            // Find the sources
            var sources = allSources.Select(x => mapping.SourceNames.Contains(x.Name)).ToList();

            if (sources.Count != mapping.SourceNames.Count())
            {
                // TODO - replace with a "mapping" exception or some such
                // TODO - better error message - tell 'em which source(s) cannot be found.
                throw new ParserException($"Could not find all sources for mapping.");
            }

            // Process all the mappings
            foreach (var entry in mapping.Entries)
            {
                // Find the item in the sink
                // TODO - rename entry.SinkItem to entry.SinkName
                var sinkItem = FindItem(sink, entry.SinkItem);

                // Find the item in the sources
                // TODO - find item in sources

                // Create the mapping between them
                // TODO - how to store mappings?
            }

            // TODO
            logger.LogWarning("-> Process is not yet implemented!");
        }


        private XsdItem FindItem(XsdDocument doc, string name)
        {
            var elem = doc.Elements.FirstOrDefault(x => x.Name == name);

            if (elem != null)
            {
                return elem;
            }

            var att = doc.Elements.SelectMany(x => x.Attributes).FirstOrDefault(x => x.Name == name);

            if (att != null)
            {
                return att;
            }

            // TODO - replace with a "mapping" exception or some such
            throw new ParserException($"Could not find item '{name}' in {doc.Name}.");
        }
    }
}
