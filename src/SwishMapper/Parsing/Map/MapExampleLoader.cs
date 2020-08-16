
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapExampleLoader : IMapExampleLoader
    {
        private const int MaxExampleLength = 100;

        private readonly ILogger logger;

        public MapExampleLoader(ILogger<MapExampleLoader> logger)
        {
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

                            // TODO - split this out to a separate class? Easier testing, and perhaps ability to handle JSON?
                            var doc = new XmlDocument();

                            doc.LoadXml(builder.ToString());

                            foreach (var attribute in exampleAttributes)
                            {
                                var nodes = doc.SelectNodes(attribute.XPath);

                                // If we didn't find a match, move on.
                                if (nodes.Count == 0)
                                {
                                    continue;
                                }

                                // If we found one match, use it. If we found multiples, set up the suffix and use the first
                                // match that we found.
                                var oneOfN = string.Empty;
                                if (nodes.Count > 1)
                                {
                                    oneOfN = $"[1-of-{nodes.Count}] ";
                                }

                                var node = nodes[0] as XmlElement;

                                if (node == null)
                                {
                                    logger.LogWarning("In {Model} example {Id}, XPath {Path} resulted in a non-element!",
                                            model.ModelId, exampleId, attribute.XPath);
                                }
                                else
                                {
                                    var childElementCount = node.ChildNodes.OfType<XmlElement>().Count();

                                    string exampleText;
                                    if (childElementCount > 0)
                                    {
                                        logger.LogWarning("In {Model} example {Id}, XPath {Path} found a node with child elements: {Content}.",
                                                model.ModelId, exampleId, attribute.XPath, node.InnerXml);

                                        exampleText = oneOfN + "[non-terminal] " + Cleanse(node.InnerXml);
                                    }
                                    else
                                    {
                                        exampleText = oneOfN + Cleanse(node.InnerText);
                                    }

                                    if (exampleText.Length > MaxExampleLength)
                                    {
                                        exampleText = exampleText.Substring(0, MaxExampleLength - 3) + "...";
                                    }

                                    attribute.Examples.Add(exampleId, exampleText);
                                }
                            }
                        }
                    }
                }
            }
        }


        private string Cleanse(string val)
        {
            return val.Replace("\n", "\\n");
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
