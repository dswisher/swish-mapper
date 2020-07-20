
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public interface IEntityMerger
    {
        int Merge(DataEntity targetEntity, DataEntity sourceEntity);
    }
}
