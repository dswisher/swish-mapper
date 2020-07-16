
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using CsvHelper;
using Microsoft.Extensions.Logging;
using SwishMapper.Extensions;
using SwishMapper.Models.Data;
using SwishMapper.Parsing;

namespace SwishMapper.Work
{
    public class CsvLoader : IModelProducer
    {
        private readonly ITypeFactory typeFactory;
        private readonly ILogger logger;

        public CsvLoader(ITypeFactory typeFactory,
                         ILogger<CsvLoader> logger)
        {
            this.typeFactory = typeFactory;
            this.logger = logger;
        }


        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string Path { get; set; }
        public string ShortName { get; set; }
        public CsvNormalizer Input { get; set; }


        public async Task<DataModel> RunAsync()
        {
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

            var rows = await Input.RunAsync();

            foreach (var row in rows)
            {
                // Find or create the entity and attribute
                var entity = model.FindOrCreateEntity(row.EntityName, source);

                if (string.IsNullOrEmpty(row.AttributeName))
                {
                    entity.Comment = row.Comment;
                }
                else
                {
                    var attribute = entity.FindOrCreateAttribute(row.AttributeName, source);

                    int? maxLength = null;
                    if (!string.IsNullOrEmpty(row.MaxLength))
                    {
                        int i;
                        if (int.TryParse(row.MaxLength, out i))
                        {
                            maxLength = i;
                        }
                        else
                        {
                            // TODO - throw a loader exception!
                        }
                    }

                    attribute.MinOccurs = row.MinOccurs;
                    attribute.MaxOccurs = row.MaxOccurs;
                    attribute.DataType = typeFactory.Make(row.DataType, row.AttributeName, maxLength);
                    attribute.Comment = row.Comment;
                }
            }

            return model;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0}", Path);

            using (var childContext = context.Push())
            {
                Input.Dump(childContext);
            }
        }
    }
}
