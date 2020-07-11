
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public interface IPopulator
    {
        Task RunAsync(DataModel model);
        void Dump(PlanDumperContext context);
    }
}
