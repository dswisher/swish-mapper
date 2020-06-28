
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

            // Add the little header table for the doc
            root.AddTable()
                .Row()
                    .Cell("Document")
                    .Cell(doc.Name)
                .Row()
                    .Cell("Root Element")
                    .Cell(doc.RootElement.Name);

            // Add the big table of all the elements
            var table = root.AddTable()
                .Row()      // TODO - set header style, or perhaps make it a .Header() call
                    .Cell("Depth")
                    .Cell("Name")
                    .Cell("Child");

            foreach (var dataElement in doc.Elements.OrderBy(x => x.Name))
            {
                var numKids = dataElement.Attributes.Count + dataElement.Elements.Count;

                table.Row()
                    .Cell(dataElement.Depth, rowSpan: numKids + 1)
                    .Cell(dataElement.Name, rowSpan: numKids + 1);

                foreach (var attribute in dataElement.Attributes)
                {
                    table.Row()
                        .Cell(attribute.Name);
                }
            }

            return root;
        }
    }
}
