
using System.Threading.Tasks;
using System.Xml;

namespace SwishMapper.Sampling
{
    public static class XmlReaderExtensions
    {
        public static async Task SkipToElementAsync(this XmlReader reader)
        {
            do
            {
                await reader.ReadAsync();
            }
            while (reader.NodeType != XmlNodeType.Element);
        }


        public static void VerifyStartElement(this XmlReader reader, string name = null)
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                throw new SamplerException($"Expected '{name}' element, but found {reader.NodeType}.");
            }

            if ((name != null) && (reader.Name != name))
            {
                throw new SamplerException($"Expected '{name}' element, but found '{reader.Name}' element instead.");
            }
        }


        public static void VerifyEndElement(this XmlReader reader, string name = null)
        {
            if (reader.NodeType != XmlNodeType.EndElement)
            {
                throw new SamplerException($"Expected '{name}' end-element, but found {reader.NodeType}.");
            }

            if (reader.Name != name)
            {
                throw new SamplerException($"Expected '{name}' end-element, but found '{reader.Name}' end-element instead.");
            }
        }


        public static async Task SkipCurrentElementAsync(this XmlReader reader)
        {
            var name = reader.Name;

            if (reader.IsEmptyElement)
            {
                return;
            }

            var depth = 1;

            while (true)
            {
                await reader.ReadAsync();

                if (reader.Name == name)
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (!reader.IsEmptyElement)
                            {
                                depth += 1;
                            }

                            break;

                        case XmlNodeType.EndElement:
                            depth -= 1;
                            if (depth == 0)
                            {
                                return;
                            }

                            break;

                        default:
                            throw new SamplerException($"Unexpected NodeType, {reader.NodeType}, during SkipCurrentElement!");
                    }
                }
                else if (reader.EOF)
                {
                    throw new SamplerException("Hit EOF during SkipCurrentElement!");
                }
            }
        }
    }
}
