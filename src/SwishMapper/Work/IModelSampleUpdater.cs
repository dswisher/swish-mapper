
namespace SwishMapper.Work
{
    public interface IModelSampleUpdater : IModelUpdater
    {
        SampleWriter Writer { get; set; }
        string SampleId { get; set; }
    }
}
