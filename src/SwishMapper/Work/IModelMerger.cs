
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    /// <summary>
    /// A worker that merges content into an already existing model.
    /// </summary>
    public interface IModelMerger
    {
        IModelProducer Input { get; set; }

        Task RunAsync(DataModel model);
        void Dump(PlanDumperContext context);
    }
}
