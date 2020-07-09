
using System.IO;

using SwishMapper.Reports;

namespace SwishMapper.Formatters
{
    public class ConsoleFormatter : IDocFormatter
    {
        private readonly TextWriter writer;

        public ConsoleFormatter(TextWriter writer)
        {
            this.writer = writer;
        }


        public void Write(IOldReport report)
        {
            var root = report.Build();

            Write(root);
        }


        public void Write(DocRoot root)
        {
            foreach (var child in root.Children)
            {
                Write((dynamic)child);
            }
        }


        private void Write(DocTable table)
        {
            foreach (var row in table.Rows)
            {
                Write(row);
            }
        }


        private void Write(DocTableRow row)
        {
            writer.Write("| ");

            foreach (var cell in row.Cells)
            {
                Write(cell);
                writer.Write(" | ");
            }

            writer.WriteLine();
        }


        private void Write(DocTableCell cell)
        {
            writer.Write(cell.Text);
        }
    }
}
