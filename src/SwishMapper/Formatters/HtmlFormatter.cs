
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


        public void Write(IOldReport report)
        {
            var root = report.Build();

            Write(root);
        }


        public void Write(DocRoot root)
        {
            writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/MarkUp/DTD/xhtml11.dtd\">");
            writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");

            writer.WriteLine("<head>");
            writer.WriteLine("<title>{0}</title>", root.Title ?? "Document of Stuff");

            if (root.Style != null)
            {
                writer.WriteLine("<link rel=\"stylesheet\" href=\"{0}\">", root.Style);
            }

            writer.WriteLine("</head><body>");

            foreach (var child in root.Children)
            {
                Write((dynamic)child);
            }

            writer.WriteLine("</body></html>");
        }


        private void Write(DocTable table)
        {
            writer.WriteLine("<table>");

            foreach (var row in table.Rows)
            {
                Write(row);
            }

            writer.WriteLine("</table>");
        }


        private void Write(DocTableRow row)
        {
            writer.Write("<tr>");

            foreach (var cell in row.Cells)
            {
                Write(cell);
            }

            writer.WriteLine("</tr>");
        }


        private void Write(DocTableCell cell)
        {
            writer.Write("<td>");
            writer.Write(cell.Text);
            writer.Write("</td>");
        }
    }
}
