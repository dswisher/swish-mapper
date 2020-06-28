
using System.Collections.Generic;

namespace SwishMapper.Formatters
{
    public class DocTable : DocNode
    {
        private readonly List<DocTableRow> rows = new List<DocTableRow>();

        public IList<DocTableRow> Rows { get { return rows; } }
    }
}
