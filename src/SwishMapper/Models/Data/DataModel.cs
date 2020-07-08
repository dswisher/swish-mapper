
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A single data model, typically one of many in a project.
    /// </summary>
    public class DataModel
    {
        private readonly Dictionary<string, DataEntity> elements = new Dictionary<string, DataEntity>();
    }
}
