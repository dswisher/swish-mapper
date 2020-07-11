
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;
using SwishMapper.Parsing.Xsd;

namespace SwishMapper.Work
{
    public class XsdPopulator : IPopulator
    {
        private readonly IXsdParser parser;
        private readonly ILogger logger;

        public XsdPopulator(IXsdParser parser,
                            ILogger<XsdPopulator> logger)
        {
            this.parser = parser;
            this.logger = logger;
        }


        public string Path { get; set; }
        public string RootElement { get; set; }


        public async Task RunAsync(DataModel model)
        {
            logger.LogWarning("XsdPopulator.RunAsync: {Path} -> still a WIP!", Path);

            var source = new DataModelSource
            {
                ShortName = "xsd",      // TODO - xyzzy - have project planner set the short name!
                Path = Path
            };

            model.Sources.Add(source);

            // Parse the XML schema document
            // TODO - remove docName as a parser parameter
            var xsdDoc = await parser.ParseAsync(Path, model.Name, RootElement, string.Empty);

            // Merge the schema info into the data model
            foreach (var xsdElement in xsdDoc.Elements)
            {
                // If this element is a simple one, skip it. While we could look at the data
                // type, instead we make sure it has at least on attribute and/or one child element.
                if ((xsdElement.Attributes.Count == 0) && (xsdElement.Elements.Count == 0))
                {
                    continue;
                }

                // Find or create the entity
                var entity = model.FindOrCreateEntity(xsdElement.Name, source);

                // Add the attributes as attributes
                foreach (var xsdChild in xsdElement.Attributes)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name);

                    // TODO - if the datatype is set and conflicts, log a warning?
                    attribute.DataType = xsdChild.DataType;

                    // TODO - set other properties: minOccurs, maxOccurs, etc.
                }

                // Add the child elements as attributes with complex types
                foreach (var xsdChild in xsdElement.Elements)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name);

                    attribute.DataType = xsdChild.DataType;
                    attribute.MinOccurs = xsdChild.MinOccurs;
                    attribute.MaxOccurs = xsdChild.MaxOccurs;

                    // TODO - set attribute properties
                }
            }

            logger.LogDebug("XsdPopulator, {Path}, model now has {Num} elements.", Path, model.Entities.Count());
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0}", Path);
        }
    }
}
