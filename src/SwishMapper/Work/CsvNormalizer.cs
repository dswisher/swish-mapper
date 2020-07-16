
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

using CsvHelper;
using Microsoft.Extensions.Logging;
using SwishMapper.Extensions;
using SwishMapper.Models;

namespace SwishMapper.Work
{
    public class CsvNormalizer
    {
        private readonly ILogger logger;

        public CsvNormalizer(ILogger<CsvNormalizer> logger)
        {
            this.logger = logger;
        }


        public string Path { get; set; }


        public async Task<List<CsvRow>> RunAsync()
        {
            var rows = new List<CsvRow>();

            using (var reader = new StreamReader(Path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // TODO - for a first pass, I'm hard-coding the setup required to read a file for my
                // specific situation. It can be generalized later.
                // This is all a HACK!

                // Skip two header rows.
                await csv.ReadAsync();
                await csv.ReadAsync();

                // Keep track of the prior row, initializing it with an empty row.
                var priorRow = new CsvRow();

                // Keep reading rows until we run out...
                while (await csv.ReadAsync())
                {
                    var row = new CsvRow();
                    rows.Add(row);

                    // If we have an entity name, update it, otherwise keep using the last seen name.
                    row.EntityName = csv.GetField(0).Crush();
                    if (string.IsNullOrEmpty(row.EntityName))
                    {
                        row.EntityName = priorRow.EntityName;
                    }

                    // Attributes could be in one of two columns.
                    row.AttributeName = csv.GetField(1).Crush();
                    if (string.IsNullOrEmpty(row.AttributeName))
                    {
                        row.AttributeName = csv.GetField(2).Crush();
                    }

                    // The remaining properties are straightforward
                    row.DataType = csv.GetField(3).Trim();
                    row.MaxLength = csv.GetField(4).Trim();
                    row.Required = csv.GetField(5).Trim();
                    row.MinOccurs = csv.GetField(6).Trim();
                    row.MaxOccurs = csv.GetField(7).Trim();
                    row.EnumValues = csv.GetField(8).Trim();
                    row.Comment = csv.GetField(9).Trim();

                    // Remember this row for next time
                    priorRow = row;
                }
            }

            // Return the list we've built.
            return rows;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this, "{0}", Path);
        }
    }
}
