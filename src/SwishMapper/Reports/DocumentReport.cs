
using System.Linq;

using SwishMapper.Formatters;
using SwishMapper.Models;

namespace SwishMapper.Reports
{
    public class DocumentReport : IOldReport
    {
        private readonly XsdDocument doc;

        public DocumentReport(XsdDocument doc)
        {
            this.doc = doc;
        }


        public DocRoot Build()
        {
            // Create the document
            var root = new DocRoot
            {
                Title = doc.Name,
                Style = "style.css"
            };

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
                    .Cell("Child")
                    .Cell("Kind");

            foreach (var dataElement in doc.Elements.OrderBy(x => x.Name))
            {
                var numKids = dataElement.Attributes.Count + dataElement.Elements.Count;

                table.Row()
                    .Cell(dataElement.Depth, rowSpan: numKids + 1)
                    .Cell(dataElement.Name, rowSpan: numKids + 1)
                    .Cell()
                    .Cell();

                foreach (var attribute in dataElement.Attributes.OrderBy(x => x.Name))
                {
                    table.Row()
                        .Cell()
                        .Cell()
                        .Cell(attribute.Name)
                        .Cell("attribute");
                }

                foreach (var elem in dataElement.Elements.OrderBy(x => x.Name))
                {
                    table.Row()
                        .Cell()
                        .Cell()
                        .Cell(elem.Name)
                        .Cell("element");
                }
            }

            return root;
        }
    }
}
