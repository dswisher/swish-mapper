
using SwishMapper.Models;

namespace SwishMapper.Work
{
    public interface IXsdToModelTranslator : IModelProducer
    {
        IWorker<XsdDocument> Input { get; set; }

        string ModelId { get; set; }
        string ModelName { get; set; }
        string Path { get; set; }
        string ShortName { get; set; }
    }
}
