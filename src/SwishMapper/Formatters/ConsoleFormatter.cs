
using System;

using SwishMapper.Reports;

namespace SwishMapper.Formatters
{
    public class ConsoleFormatter
    {
        public void Write(IReport report)
        {
            var root = report.Build();

            Write(root);
        }


        private void Write(DocRoot root)
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
                Console.WriteLine(row.Text);
            }
        }
    }
}
