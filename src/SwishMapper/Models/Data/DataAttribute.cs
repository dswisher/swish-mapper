
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// A named bit of data inside an entity.
    /// </summary>
    public class DataAttribute
    {
        private readonly List<DataAttributeMap> incomingMaps = new List<DataAttributeMap>();
        private readonly List<DataAttributeMap> outgoingMaps = new List<DataAttributeMap>();
        private readonly List<DataModelSource> sources = new List<DataModelSource>();
        private readonly List<DataAttributeSample> samples = new List<DataAttributeSample>();


        public DataAttribute(DataEntity parent, string name)
        {
            Parent = parent;
            Name = name;
        }

        /// <summary>
        /// The parent entity of this attribute.
        /// </summary>
        public DataEntity Parent { get; private set; }

        /// <summary>
        /// The name of this attribute.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The data type of this attribute.
        /// </summary>
        public DataType DataType { get; set; }

        public string MinOccurs { get; set; }
        public string MaxOccurs { get; set; }

        public string Comment { get; set; }

        public List<DataAttributeMap> IncomingMaps { get { return incomingMaps; } }
        public List<DataAttributeMap> OutgoingMaps { get { return outgoingMaps; } }
        public List<DataAttributeSample> Samples { get { return samples; } }

        /// <summary>
        /// The sources used to determine the definition of this attribtue.
        /// </summary>
        public IList<DataModelSource> Sources { get { return sources; } }
    }
}
