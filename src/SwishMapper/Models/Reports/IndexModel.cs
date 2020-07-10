
using System.Collections.Generic;

namespace SwishMapper.Models.Reports
{
    public class IndexModel
    {
        private readonly List<IndexModelSection> sections = new List<IndexModelSection>();

        public IList<IndexModelSection> Sections { get { return sections; } }

        public IndexModelSection CreateSection(string name)
        {
            var section = new IndexModelSection
            {
                Name = name
            };

            sections.Add(section);

            return section;
        }
    }
}
