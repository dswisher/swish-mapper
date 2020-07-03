
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// Details of a mapping between a sink and one or more related sources.
    /// </summary>
    public class Mapping
    {
        private readonly List<DataDocument> sources = new List<DataDocument>();
        private readonly List<MappingEntry> entries = new List<MappingEntry>();


        /// <summary>
        /// The document whose attributes are populated (derived, copied) from the source documents, below.
        /// <summary>
        public DataDocument Sink { get; set; }


        /// <summary>
        /// The mapping entries found during the parse.
        /// <summary>
        public IEnumerable<MappingEntry> Entries { get { return entries; } }


        /// <summary>
        /// The source documents used to populate the sink, above.
        /// <summary>
        public IEnumerable<DataDocument> Sources { get { return sources; } }


        /// <summary>
        /// Add a source to the mapping.
        /// <summary>
        public void AddSource(DataDocument source)
        {
            sources.Add(source);
        }


        /// <summary>
        /// Add an entry.
        /// <summary>
        public void AddEntry(MappingEntry entry)
        {
            entries.Add(entry);
        }


        /// <summary>
        /// Convenience method to create and add an entry.
        /// <summary>
        public void AddEntry(DataItem source, DataItem sink)
        {
            var entry = new MappingEntry
            {
                SourceItem = source,
                SinkItem = sink
            };

            entries.Add(entry);
        }
    }
}
