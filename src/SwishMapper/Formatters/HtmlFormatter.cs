
using System.IO;

using SwishMapper.Reports;

namespace SwishMapper.Formatters
{
    public class HtmlFormatter : IDocFormatter
    {
        private readonly TextWriter writer;

        public HtmlFormatter(TextWriter writer)
        {
            this.writer = writer;
        }


        public void Write(IReport report)
        {
            var root = report.Build();

            Write(root);
        }


        public void Write(DocRoot root)
        {
            writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/MarkUp/DTD/xhtml11.dtd\">");
            writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");

            foreach (var child in root.Children)
            {
                Write((dynamic)child);
            }

            writer.WriteLine("</html>");
        }


        private void Write(DocTable table)
        {
            // TODO - this is WRONG! Actually build proper XHTML! Hoping to get the unit test finished, first.

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
