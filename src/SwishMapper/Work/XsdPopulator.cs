
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;
using SwishMapper.Parsing;

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
            logger.LogDebug("XsdPopulator.RunAsync: {Path} -> not yet implemented!", Path);

            // Parse the XML schema document
            // TODO - remove docName as a parser parameter
            var xsdDoc = await parser.ParseAsync(Path, model.Name, RootElement, string.Empty);

            // Merge the schema info into the data model
            foreach (var elem in xsdDoc.Elements)
            {
                var entity = model.FindOrCreateEntity(elem.Name);

                // TODO - merge the XSD info into the data model
            }

            logger.LogDebug("XsdPopulator, {Path}, model now has {Num} elements.", Path, model.Entities.Count());
        }
    }
}
