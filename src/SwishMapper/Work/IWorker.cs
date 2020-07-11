
using System.Threading.Tasks;

namespace SwishMapper.Work
{
    public interface IWorker<T>
    {
        Task<T> RunAsync();
        void Dump(PlanDumperContext context);
    }
}
