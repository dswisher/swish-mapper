
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Microsoft.Extensions.Logging;
using SwishMapper.Models.Data;

namespace SwishMapper.Parsing.Map
{
    public class MapExampleXmlLoader : IMapExampleXmlLoader
    {
        private const int MaxExampleLength = 100;

        private readonly ILogger logger;

        public MapExampleXmlLoader(ILogger<MapExampleXmlLoader> logger)
        {
            this.logger = logger;
        }


        public void LoadExamples(string modelId, string content, IEnumerable<MappedDataAttribute> attributes, string exampleId)
        {
            var doc = new XmlDocument();

            doc.LoadXml(content);

            foreach (var attribute in attributes)
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

                var xmlElement = nodes[0] as XmlElement;
                var xmlAttribute = nodes[0] as XmlAttribute;

                if (xmlElement != null)
                {
                    var childElementCount = xmlElement.ChildNodes.OfType<XmlElement>().Count();

                    string exampleText;
                    if (childElementCount > 0)
                    {
                        logger.LogWarning("In {Model} example {Id}, XPath {Path} found a node with child elements: {Content}.",
                                modelId, exampleId, attribute.XPath, xmlElement.InnerXml);

                        exampleText = oneOfN + "[non-terminal] " + Cleanse(xmlElement.InnerXml);
                    }
                    else
                    {
                        exampleText = oneOfN + Cleanse(xmlElement.InnerText);
                    }

                    attribute.Examples.Add(exampleId, ClipString(exampleText));
                }
                else if (xmlAttribute != null)
                {
                    attribute.Examples.Add(exampleId, ClipString(xmlAttribute.Value));
                }
                else
                {
                    logger.LogWarning("In {Model} example {Id}, XPath {Path} resulted in a non-element!",
                            modelId, exampleId, attribute.XPath);
                }
            }
        }


        private string ClipString(string val)
        {
            if (val.Length > MaxExampleLength)
            {
                return val.Substring(0, MaxExampleLength - 3) + "...";
            }

            return val;
        }


        private string Cleanse(string val)
        {
            return val.Replace("\n", "\\n");
        }
    }
}
