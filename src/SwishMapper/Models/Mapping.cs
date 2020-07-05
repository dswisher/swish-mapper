
using System.Collections.Generic;

namespace SwishMapper.Models
{
    /// <summary>
    /// Details of a mapping between a sink and one or more related sources.
    /// </summary>
    public class Mapping
    {
        private readonly List<string> sourceNames = new List<string>();
        private readonly List<MappingEntry> entries = new List<MappingEntry>();


        /// <summary>
        /// The name of the sink for this mapping document.
        /// <summary>
        public string SinkName { get; set; }


        /// <summary>
        /// The mapping entries found during the parse.
        /// <summary>
        public IEnumerable<MappingEntry> Entries { get { return entries; } }


        /// <summary>
        /// The source documents used to populate the sink, above.
        /// <summary>
        public IEnumerable<string> SourceNames { get { return sourceNames; } }


        /// <summary>
        /// Add a source name to the mapping.
        /// <summary>
        public void AddSourceName(string source)
        {
            sourceNames.Add(source);
        }


        /// <summary>
        /// Add an entry.
        /// <summary>
        public void AddEntry(MappingEntry entry)
        {
            entries.Add(entry);
        }
    }
}
