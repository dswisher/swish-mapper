
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public interface IModelUpdater
    {
        Task RunAsync(DataModel model);
        void Dump(PlanDumperContext context);
    }
}
