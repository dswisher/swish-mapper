
namespace SwishMapper.Formatters
{
    public static class DocExtensions
    {
        public static DocTable AddTable(this DocRoot root)
        {
            var table = new DocTable();

            root.Children.Add(table);

            return table;
        }


        public static DocTableRow Row(this DocTable table)
        {
            return new DocTableRow(table);
        }


        public static DocTableRow Row(this DocTableRow predecessor)
        {
            return new DocTableRow(predecessor.Table);
        }


        public static DocTableRow Cell<T>(this DocTableRow row, T content, int rowSpan = 0)
        {
            var cell = new DocTableCell
            {
                Text = content.ToString(),
                RowSpan = rowSpan
            };

            row.Cells.Add(cell);

            return row;
        }
    }
}
