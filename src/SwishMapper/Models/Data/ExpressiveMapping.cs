
namespace SwishMapper.Models.Data
{
    public class ExpressiveMapping
    {
        /// <summary>
        /// The attribute that is being assigned; the left-hand side; the "sink".
        /// </summary>
        public MappedDataAttribute TargetAttribute { get; set; }

        /// <summary>
        /// The expression that defines the value to assign; the right-hand side; the "source".
        /// </summary>
        public MappedDataExpression Expression { get; set; }
    }
}
