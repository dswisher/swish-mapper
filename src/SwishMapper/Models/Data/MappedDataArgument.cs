
namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A single argument on the right-hand-side (RHS) of an expression.
    /// </summary>
    /// <remarks>
    /// Only one of the values will ever be set.
    /// </remarks>
    public class MappedDataArgument
    {
        // TODO - would it be better to break these into multiple classes with a base interface/class?

        public MappedDataArgument(MappedDataAttribute attribute)
        {
            Attribute = attribute;
        }

        public MappedDataArgument(DataModel model)
        {
            Model = model;
        }

        /// <summary>
        /// An attribute within a data model.
        /// </summary>
        public MappedDataAttribute Attribute { get; private set; }

        /// <summary>
        /// A data model.
        /// </summary>
        public DataModel Model { get; private set; }

        // TODO - add literals
        // TODO - implement ToString()
    }
}
