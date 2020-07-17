
using System.Collections.Generic;
using System.Threading.Tasks;

using SwishMapper.Models;

namespace SwishMapper.Work
{
    public interface ICsvNormalizer
    {
        string Path { get; set; }

        Task<List<CsvRow>> RunAsync();
        void Dump(PlanDumperContext context);
    }
}
