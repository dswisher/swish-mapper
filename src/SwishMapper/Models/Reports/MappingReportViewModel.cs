
using System.Collections.Generic;

using SwishMapper.Extensions;
using SwishMapper.Models.Data;

namespace SwishMapper.Models.Reports
{
    public class MappingReportViewModel
    {
        private readonly Dictionary<string, MappingReportModel> models = new Dictionary<string, MappingReportModel>();

        public string Name { get; set; }

        public IEnumerable<MappingReportModel> Models { get { return models.Values; } }


        public MappingReportModel FindOrCreateModel(DataModel dataModel)
        {
            return models.FindOrCreate(dataModel.Id, () => new MappingReportModel
            {
                Id = dataModel.Id,
                Name = dataModel.Name
            });
        }
    }
}
