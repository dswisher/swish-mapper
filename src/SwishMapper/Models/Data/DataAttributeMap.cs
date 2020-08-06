
namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A mapping between two attributes.
    /// </summary>
    // TODO - xyzzy - obsolete - get rid of this
    public class DataAttributeMap
    {
        /// <summary>
        /// The source of the mapping.
        /// </summary>
        public DataAttribute FromAttribute { get; set; }

        /// <summary>
        /// The source xpath.
        /// </summary>
        public string FromXPath { get; set; }

        /// <summary>
        /// The link to this attribute in the from data-model.
        /// </summary>
        public string FromUrl { get; set; }

        /// <summary>
        /// The sink of the mapping.
        /// </summary>
        public DataAttribute ToAttribute { get; set; }

        /// <summary>
        /// The sink xpath.
        /// </summary>
        public string ToXPath { get; set; }

        /// <summary>
        /// Comments about this mapping.
        /// </summary>
        public string Comments { get; set; }
    }
}
