
namespace SwishMapper.Work
{
    public interface IEmptyEntityCleaner : IModelProducer
    {
        IModelProducer Input { get; set; }
    }
}
