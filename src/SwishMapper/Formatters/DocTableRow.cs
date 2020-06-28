
using System.Collections.Generic;

namespace SwishMapper.Formatters
{
    public class DocTableRow
    {
        private readonly List<DocTableCell> cells = new List<DocTableCell>();

        public DocTableRow(DocTable table)
        {
            Table = table;

            Table.Rows.Add(this);
        }

        public DocTable Table { get; private set; }
        public IList<DocTableCell> Cells { get { return cells; } }
    }
}
