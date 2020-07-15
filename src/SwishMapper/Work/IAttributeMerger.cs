
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public interface IAttributeMerger
    {
        int Merge(DataAttribute targetAttribute, DataAttribute sourceAttribute);
    }
}
