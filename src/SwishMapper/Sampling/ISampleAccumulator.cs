
namespace SwishMapper.Sampling
{
    public interface ISampleAccumulator
    {
        void Push(string name);
        void Pop();
    }
}
