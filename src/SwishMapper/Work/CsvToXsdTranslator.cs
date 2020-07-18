
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;

namespace SwishMapper.Work
{
    public class CsvToXsdTranslator : ICsvToXsdTranslator
    {
        private readonly ILogger logger;

        public CsvToXsdTranslator(ILogger<CsvToXsdTranslator> logger)
        {
            this.logger = logger;
        }


        public ICsvNormalizer Input { get; set; }
        public string DebugDumpPath { get; set; }


        public async Task<XsdDocument> RunAsync()
        {
            var rows = await Input.RunAsync();
            var doc = new XsdDocument();

            // Process all the input rows
            foreach (var row in rows)
            {
                var element = doc.FindOrCreateElement(row.ElementName);

                if (!string.IsNullOrEmpty(row.ChildElementName))
                {
                    // This row applies to a child element...
                    var child = element.FindOrCreateElement(row.ChildElementName);

                    child.DataType = row.DataType;
                    child.MinOccurs = row.MinOccurs;
                    child.MaxOccurs = row.MaxOccurs;

                    if (child.DataType == "ref")
                    {
                        child.RefName = row.ChildElementName;
                    }
                }
                else if (!string.IsNullOrEmpty(row.AttributeName))
                {
                    // This row applies to a child attribute...
                    var attribute = element.FindOrCreateAttribute(row.AttributeName);

                    attribute.DataType = row.DataType;
                    attribute.MaxLength = row.MaxLength;
                    attribute.Comment = row.Comment;

                    if (!string.IsNullOrEmpty(row.Required))
                    {
                        attribute.MaxOccurs = "1";
                        if (row.Required.Equals("required", StringComparison.OrdinalIgnoreCase))
                        {
                            attribute.MinOccurs = "1";
                        }
                        else
                        {
                            attribute.MinOccurs = "0";
                        }
                    }
                }
                else
                {
                    // This row applies to the element itself
                    element.DataType = row.DataType;
                    element.MaxLength = row.MaxLength;
                    element.Comment = row.Comment;
                }
            }

            // If a debug path was specified, write the doc there for post-mortem analysis...
            if (!string.IsNullOrEmpty(DebugDumpPath))
            {
                using (var stream = new FileStream(DebugDumpPath, FileMode.Create, FileAccess.Write))
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    await JsonSerializer.SerializeAsync(stream, doc, jsonOptions);
                }
            }

            // Return what we've built
            return doc;
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
