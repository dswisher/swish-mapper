
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;
using SwishMapper.Parsing.Xsd;

namespace SwishMapper.Work
{
    public class XsdLoader : IModelProducer
    {
        private readonly IXsdParser parser;
        private readonly ILogger logger;

        public XsdLoader(IXsdParser parser,
                         ILogger<CsvLoader> logger)
        {
            this.parser = parser;
            this.logger = logger;
        }


        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string Path { get; set; }
        public string RootElement { get; set; }


        public async Task<DataModel> RunAsync()
        {
            var model = new DataModel
            {
                Id = ModelId,
                Name = ModelName
            };

            var source = new DataModelSource
            {
                ShortName = "xsd",      // TODO - xyzzy - have project planner set the short name!
                Path = Path
            };

            model.Sources.Add(source);

            // Parse the XML schema document
            // TODO - remove docName as a parser parameter
            // TODO - remove RootElement as a parser parameter - just return ALL elements
            var xsdDoc = await parser.ParseAsync(Path, model.Name, RootElement, string.Empty);

            // Populate the model from the parsed schema
            foreach (var xsdElement in xsdDoc.Elements)
            {
                // TODO - still want to skip elements? Leave that for cleanup?
                // If this element is a simple one, skip it. While we could look at the data
                // type, instead we make sure it has at least on attribute and/or one child element.
                // if ((xsdElement.Attributes.Count == 0) && (xsdElement.Elements.Count == 0))
                // {
                //     continue;
                // }

                // Find or create the entity
                var entity = model.FindOrCreateEntity(xsdElement.Name, source);

                // Add the attributes as attributes
                foreach (var xsdChild in xsdElement.Attributes)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name, source);

                    attribute.DataType = xsdChild.DataType;

                    // TODO - set other properties: max-length, required, etc.
                }

                // Add the child elements as attributes with complex types
                foreach (var xsdChild in xsdElement.Elements)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name, source);

                    attribute.DataType = xsdChild.DataType;
                    attribute.MinOccurs = xsdChild.MinOccurs;
                    attribute.MaxOccurs = xsdChild.MaxOccurs;

                    // TODO - set attribute properties
                }
            }

            return model;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0}", Path);
        }
    }
}
