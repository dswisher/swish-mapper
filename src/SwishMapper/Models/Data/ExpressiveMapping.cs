
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class ExpressiveMapping
    {
        private readonly List<string> notes = new List<string>();

        public ExpressiveMapping(string id, MappedDataAttribute targetAttribute, MappedDataExpression expression)
        {
            Id = id;
            TargetAttribute = targetAttribute;
            Expression = expression;
        }

        /// <summary>
        /// A unique identifier for this mapping within its attribute.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The attribute that is being assigned; the left-hand side; the "sink".
        /// </summary>
        public MappedDataAttribute TargetAttribute { get; private set; }

        /// <summary>
        /// The expression that defines the value to assign; the right-hand side; the "source".
        /// </summary>
        public MappedDataExpression Expression { get; private set; }

        /// <summary>
        /// Any notes associated with this mapping.
        /// </summary>
        public IList<string> Notes { get { return notes; } }
    }
}
