
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapExampleLoader : IMapExampleLoader
    {
        private const int MaxExampleLength = 100;

        private readonly IMapExampleXmlLoader xmlLoader;
        private readonly ILogger logger;

        public MapExampleLoader(IMapExampleXmlLoader xmlLoader,
                                ILogger<MapExampleLoader> logger)
        {
            this.xmlLoader = xmlLoader;
            this.logger = logger;
        }


        public async Task LoadAsync(MapParserContext context)
        {
            var mapDir = Path.GetDirectoryName(context.FilePath);

            // Go through all the examples
            foreach (var def in context.Examples)
            {
                foreach (var model in def.Models)
                {
                    // Find all the attributes from this model within the mapping
                    var exampleAttributes = FindAttributesForModel(context.MapList, model.ModelId);

                    // Process each directory where examples are located
                    foreach (var dir in model.Directories)
                    {
                        var dirPath = Path.Combine(mapDir, dir);
                        var directory = new DirectoryInfo(dirPath);

                        foreach (var fileInfo in directory.GetFiles())
                        {
                            var exampleId = Path.GetFileNameWithoutExtension(fileInfo.Name);

                            logger.LogDebug("Loading {Model} sample {Id}...", model.ModelId, exampleId);

                            var builder = new StringBuilder();

                            builder.Append(model.Prefix);

                            using (var reader = fileInfo.OpenText())
                            {
                                builder.Append(await reader.ReadToEndAsync());
                            }

                            builder.Append(model.Suffix);

                            xmlLoader.LoadExamples(model.ModelId, builder.ToString(), exampleAttributes, exampleId);
                        }
                    }
                }
            }
        }


        private IEnumerable<MappedDataAttribute> FindAttributesForModel(ExpressiveMapList mapList, string modelId)
        {
            foreach (var mapping in mapList.Maps)
            {
                if (BelongsToModel(mapping.TargetAttribute, modelId))
                {
                    yield return mapping.TargetAttribute;
                }

                foreach (var arg in mapping.Expression.Arguments)
                {
                    if ((arg.Attribute != null) && BelongsToModel(arg.Attribute, modelId))
                    {
                        yield return arg.Attribute;
                    }
                }
            }
        }


        private bool BelongsToModel(MappedDataAttribute attribute, string modelId)
        {
            return attribute.Attribute.Parent.Parent.Id == modelId;
        }
    }
}
