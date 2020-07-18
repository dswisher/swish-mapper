
using System.Threading.Tasks;
using System.Xml;

using Microsoft.Extensions.Logging;

namespace SwishMapper.Sampling
{
    public class XmlSampler : IXmlSampler
    {
        private readonly ILogger logger;

        public XmlSampler(ILogger<XmlSampler> logger)
        {
            this.logger = logger;
        }


        public async Task SampleAsync(SampleStream stream, ISampleAccumulator accumulator)
        {
            var settings = new XmlReaderSettings
            {
                Async = true,
                CloseInput = false,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            };

            using (var reader = XmlReader.Create(stream.Stream, settings))
            {
                await reader.SkipToElementAsync();

                await ProcessElementAsync(reader, accumulator);
            }
        }


        private async Task ProcessElementAsync(XmlReader reader, ISampleAccumulator accumulator)
        {
            // We should be sitting on an Element
            reader.VerifyStartElement();

            var name = reader.Name;

            accumulator.Push(name);

            // If there are attributes, process 'em
            ProcessAttributes(reader, accumulator);

            // Empty elements (<foo/>) don't have children.
            if (!reader.IsEmptyElement)
            {
                // Advance to the first child element
                await reader.ReadAsync();

                // Process any child elements, until we get to an end
                var done = false;
                while (!done)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            await ProcessElementAsync(reader, accumulator);
                            await reader.ReadAsync();
                            break;

                        case XmlNodeType.Text:
                            accumulator.SetValue(reader.Value);
                            await reader.ReadAsync();
                            break;

                        case XmlNodeType.EndElement:
                            done = true;
                            break;

                        default:
                            throw new SamplerException($"Unexpected node type: {reader.NodeType}.");
                    }
                }

                reader.VerifyEndElement(name);
            }

            // We're heading back up to the parent, so pop
            accumulator.Pop();
        }


        private void ProcessAttributes(XmlReader reader, ISampleAccumulator accumulator)
        {
            if (reader.HasAttributes)
            {
                // Go through all the attributes
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name.StartsWith("xmlns:"))
                    {
                        continue;
                    }

                    if (reader.Name == "xsi:nil")
                    {
                        continue;
                    }

                    accumulator.Push(reader.Name, isAttribute: true);
                    accumulator.SetValue(reader.Value);
                    accumulator.Pop();
                }

                // Move back to the element node
                reader.MoveToElement();
            }
        }
    }
}
