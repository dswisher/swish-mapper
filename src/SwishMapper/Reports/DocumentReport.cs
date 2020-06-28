
using System.Linq;

using SwishMapper.Formatters;
using SwishMapper.Models;

namespace SwishMapper.Reports
{
    public class DocumentReport : IReport
    {
        private readonly DataDocument doc;

        public DocumentReport(DataDocument doc)
        {
            this.doc = doc;
        }


        public DocRoot Build()
        {
            var root = new DocRoot();

            var table = new DocTable();

            root.Children.Add(table);

            foreach (var dataElement in doc.Elements.OrderBy(x => x.Depth).ThenBy(x => x.Name))
            {
                var row = new DocTableRow
                {
                    Text = $"{dataElement.Depth} - {dataElement.Name}"
                };

                table.Rows.Add(row);
            }

            return root;
        }
    }
}
