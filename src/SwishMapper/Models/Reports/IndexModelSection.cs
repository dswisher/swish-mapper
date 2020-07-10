
using System.Collections.Generic;

namespace SwishMapper.Models.Reports
{
    public class IndexModelSection
    {
        private readonly List<IndexModelEntry> entries = new List<IndexModelEntry>();

        public string Name { get; set; }
        public IList<IndexModelEntry> Entries { get { return entries; } }

        public IndexModelEntry CreateEntry(string name, string path)
        {
            var entry = new IndexModelEntry
            {
                Name = name,
                Path = path
            };

            entries.Add(entry);

            return entry;
        }
    }
}
