
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class CsvPopulator : IPopulator
    {
        private readonly ILogger logger;

        public CsvPopulator(ILogger<CsvPopulator> logger)
        {
            this.logger = logger;
        }


        public string Path { get; set; }


        public async Task RunAsync(DataModel model)
        {
            logger.LogWarning("CsvPopulator.RunAsync: {Path} -> still a WIP!", Path);

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
                        var attribute = entity.FindOrCreateAttribute(attributeName);

                        // TODO - if the target datatype, etc., has a value - check for conflict!

                        if (!string.IsNullOrEmpty(attribute.DataType) && (attribute.DataType != csv.GetField(3)))
                        {
                            logger.LogWarning("Datatype conflict from CSV file on {Entity}.{Attribute}: {Old} vs {New}",
                                    entityName, attributeName, attribute.DataType, csv.GetField(3));
                        }
                        else
                        {
                            attribute.DataType = csv.GetField(3);
                        }

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
        }
    }
}
