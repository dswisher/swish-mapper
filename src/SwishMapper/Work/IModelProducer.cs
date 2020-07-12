
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    /// <summary>
    /// A worker that takes a model and creates a new model from it.
    /// </summary>
    public interface IModelProducer : IWorker<DataModel>
    {
    }
}
