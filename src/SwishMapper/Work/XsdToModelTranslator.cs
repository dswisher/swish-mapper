
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;
using SwishMapper.Models.Data;
using SwishMapper.Parsing;

namespace SwishMapper.Work
{
    public class XsdToModelTranslator : IXsdToModelTranslator
    {
        private readonly ITypeFactory typeFactory;
        private readonly ILogger logger;

        public XsdToModelTranslator(ITypeFactory typeFactory,
                                    ILogger<XsdToModelTranslator> logger)
        {
            this.typeFactory = typeFactory;
            this.logger = logger;
        }


        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string Path { get; set; }
        public string ShortName { get; set; }

        public IWorker<XsdDocument> Input { get; set; }


        public async Task<DataModel> RunAsync()
        {
            // Set up the model
            var model = new DataModel
            {
                Id = ModelId,
                Name = ModelName
            };

            var source = new DataModelSource
            {
                ShortName = ShortName,
                Path = Path
            };

            model.Sources.Add(source);

            // Get the document that we'll be translating to a model
            var xsdDoc = await Input.RunAsync();

            // Populate the model
            foreach (var xsdElement in xsdDoc.Elements)
            {
                // TODO - xyzzy - need a better way to filter out the leaf elements
                // Simple elements (string, int, etc) have a datatype. It appears that complex
                // elements (for which we want to create entities) have a null datatype. So,
                // here we skip elements with a null datatype.
                // if (xsdElement.DataType != "ref")
                // {
                //     continue;
                // }

                // Find or create the entity
                var entity = model.FindOrCreateEntity(xsdElement.Name, source);

                // Add the attributes as attributes
                foreach (var xsdChild in xsdElement.Attributes)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name, source);

                    int? maxLength = null;
                    if (!string.IsNullOrEmpty(xsdChild.MaxLength))
                    {
                        int i;
                        if (int.TryParse(xsdChild.MaxLength, out i))
                        {
                            maxLength = i;
                        }
                        else
                        {
                            // TODO - throw a loader exception!
                        }
                    }

                    attribute.DataType = typeFactory.Make(xsdChild.DataType, maxLength);
                    attribute.Comment = xsdChild.Comment;
                    attribute.MinOccurs = xsdChild.MinOccurs;
                    attribute.MaxOccurs = xsdChild.MaxOccurs;
                    attribute.IsXmlAttribute = true;

                    // TODO - set other properties: max-length, required, etc.
                }

                // Add the child elements as attributes with complex types
                foreach (var xsdChild in xsdElement.Elements)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name, source);

                    if (xsdChild.DataType != null)
                    {
                        int? maxLength = null;
                        if (!string.IsNullOrEmpty(xsdChild.MaxLength))
                        {
                            int i;
                            if (int.TryParse(xsdChild.MaxLength, out i))
                            {
                                maxLength = i;
                            }
                            else
                            {
                                // TODO - throw a loader exception!
                            }
                        }

                        // TODO - throw exception if DataType is null!
                        if (string.IsNullOrEmpty(xsdChild.RefName))
                        {
                            attribute.DataType = typeFactory.Make(xsdChild.DataType, maxLength);
                        }
                        else
                        {
                            attribute.DataType = typeFactory.Make(xsdChild.DataType, xsdChild.RefName);
                        }
                    }

                    attribute.MinOccurs = xsdChild.MinOccurs;
                    attribute.MaxOccurs = xsdChild.MaxOccurs;

                    // TODO - set attribute properties
                }
            }

            // Return our glorious result
            return model;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this);

            using (var childContext = context.Push())
            {
                Input.Dump(childContext);
            }
        }
    }
}
