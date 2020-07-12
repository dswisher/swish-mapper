
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using CsvHelper;
using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class CsvLoader : IModelProducer
    {
        private readonly ILogger logger;

        public CsvLoader(ILogger<CsvLoader> logger)
        {
            this.logger = logger;
        }


        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string Path { get; set; }


        public async Task<DataModel> RunAsync()
        {
            var model = new DataModel
            {
                Id = ModelId,
                Name = ModelName
            };

            var source = new DataModelSource
            {
                ShortName = "csv",      // TODO - xyzzy - have project planner set the short name!
                Path = Path
            };

            model.Sources.Add(source);

            using (var reader = new StreamReader(Path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // TODO - for a first pass, I'm hard-coding the setup required to read a file for my
                // specific situation. It can be generalized later.
                // This is all a HACK!

                // Skip two header rows, and set up some variables.
                await csv.ReadAsync();
                await csv.ReadAsync();

                var entityName = string.Empty;

                // Keep reading rows until we run out...
                while (await csv.ReadAsync())
                {
                    // If we have an entity name, update it, otherwise keep using the last seen name.
                    if (!string.IsNullOrEmpty(csv.GetField(0)))
                    {
                        entityName = csv.GetField(0);
                    }

                    // Find or create the entity
                    var entity = model.FindOrCreateEntity(entityName, source);

                    // Attributes could be in one of two columns.
                    var attributeName = csv.GetField(1);
                    if (string.IsNullOrEmpty(attributeName))
                    {
                        attributeName = csv.GetField(2);
                    }

                    // Grab the comment (if any)
                    var comment = csv.GetField(9);

                    // If we have an attribute, set it, otherwise set the comment.
                    if (!string.IsNullOrEmpty(attributeName))
                    {
                        var attribute = entity.FindOrCreateAttribute(attributeName, source);

                        attribute.DataType = csv.GetField(3);

                        // TODO - use remaining attributes
                        // maxlength - 4
                        // attribute spec - 5 (aka required)
                        attribute.MinOccurs = csv.GetField(6);
                        attribute.MaxOccurs = csv.GetField(7);

                        // tokens - 8 (aka enum values)

                        attribute.Comment = comment;
                    }
                    else if (!string.IsNullOrEmpty(comment))
                    {
                        entity.Comment = comment;
                    }
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
