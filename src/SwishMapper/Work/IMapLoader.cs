
using System.Threading.Tasks;

using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public interface IMapLoader
    {
        string FromModelId { set; }
        string ToModelId { set; }
        string Path { set; }

        Task RunAsync(DataProject project);
        void Dump(PlanDumperContext context);
    }
}
