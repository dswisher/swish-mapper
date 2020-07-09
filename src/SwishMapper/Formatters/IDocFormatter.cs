
using SwishMapper.Reports;

namespace SwishMapper.Formatters
{
    public interface IDocFormatter
    {
        // TODO - deprecate this first method to decouple reports and formatting
        void Write(IOldReport report);

        void Write(DocRoot root);
    }
}
