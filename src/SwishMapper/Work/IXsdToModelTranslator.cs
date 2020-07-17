
namespace SwishMapper.Work
{
    public interface IXsdToModelTranslator : IModelProducer
    {
        ICsvToXsdTranslator Input { get; set; }

        string ModelId { get; set; }
        string ModelName { get; set; }
        string Path { get; set; }
        string ShortName { get; set; }
    }
}
