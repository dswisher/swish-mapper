
using System.Collections.Generic;

using SwishMapper.Models;
using SwishMapper.Models.Data;

namespace SwishMapper.Reports
{
    public interface IReportPlanner
    {
        IEnumerable<IReportWorker> CreateWork(DataProject dataProject, AppSettings settings);
    }
}
