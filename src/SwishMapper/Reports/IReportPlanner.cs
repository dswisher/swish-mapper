
using System.Collections.Generic;

using SwishMapper.Models.Data;
using SwishMapper.Models.Project;

namespace SwishMapper.Reports
{
    public interface IReportPlanner
    {
        IEnumerable<IReportWorker> CreateWork(DataProject dataProject, ProjectDefinition projectDefinition);
    }
}
