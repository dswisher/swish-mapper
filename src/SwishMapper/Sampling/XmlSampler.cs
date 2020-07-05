
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

            logger.LogDebug("Element name: {Name}", name);

            // TODO - process attributes

            // TODO - HACK! For now, just skip the element
            await reader.SkipCurrentElementAsync();

            // Make sure the end element matches what we expect
            reader.VerifyEndElement(name);

            accumulator.Pop();
        }
    }
}
