
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
                // Find or create the entity
                var entity = model.FindOrCreateEntity(xsdElement.Name, source);

                entity.Comment = xsdElement.Comment;

                if (!string.IsNullOrEmpty(xsdElement.DataType))
                {
                    if (string.IsNullOrEmpty(xsdElement.RefName))
                    {
                        entity.DataType = typeFactory.Make(xsdElement.DataType, ParseMaxLength(xsdElement.MaxLength));
                    }
                    else
                    {
                        // This should never happen, but include it anyway
                        entity.DataType = typeFactory.Make(xsdElement.DataType, xsdElement.RefName);
                    }
                }

                // Add the attributes as attributes
                foreach (var xsdChild in xsdElement.Attributes)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name, source);

                    attribute.DataType = typeFactory.Make(xsdChild.DataType, ParseMaxLength(xsdChild.MaxLength));
                    attribute.Comment = xsdChild.Comment;
                    attribute.MinOccurs = xsdChild.MinOccurs;
                    attribute.MaxOccurs = xsdChild.MaxOccurs;
                    attribute.IsXmlAttribute = true;

                    attribute.EnumValues.AddRange(xsdChild.EnumValues);
                }

                // Add the child elements as attributes with complex types
                foreach (var xsdChild in xsdElement.Elements)
                {
                    var attribute = entity.FindOrCreateAttribute(xsdChild.Name, source);

                    if (xsdChild.DataType != null)
                    {
                        if (string.IsNullOrEmpty(xsdChild.RefName))
                        {
                            attribute.DataType = typeFactory.Make(xsdChild.DataType, ParseMaxLength(xsdChild.MaxLength));
                        }
                        else
                        {
                            attribute.DataType = typeFactory.Make(xsdChild.DataType, xsdChild.RefName);
                        }
                    }
                    else
                    {
                        // TODO - xyzzy - throw exception if DataType is null!
                    }

                    attribute.Comment = xsdChild.Comment;
                    attribute.MinOccurs = xsdChild.MinOccurs;
                    attribute.MaxOccurs = xsdChild.MaxOccurs;

                    attribute.EnumValues.AddRange(xsdChild.EnumValues);
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


        private int? ParseMaxLength(string maxStr)
        {
            int? maxLength = null;

            if (!string.IsNullOrEmpty(maxStr))
            {
                int i;
                if (int.TryParse(maxStr, out i))
                {
                    maxLength = i;
                }
                else
                {
                    // TODO - throw a loader exception!
                }
            }

            return maxLength;
        }
    }
}
