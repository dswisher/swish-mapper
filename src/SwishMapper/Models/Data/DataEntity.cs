
using System.Collections.Generic;
using System.Linq;

namespace SwishMapper.Models.Data
{
    /// <summary>
    /// An entity within a data model.
    /// </summary>
    /// <remarks>
    /// Similar to a table in a database, or a complex type element in an XSD.
    /// </remarks>
    public class DataEntity
    {
        private readonly List<DataAttribute> attributes = new List<DataAttribute>();
        private readonly List<DataModelSource> sources = new List<DataModelSource>();


        public DataEntity(DataModel parent, string name)
        {
            Parent = parent;
            Name = name;
        }


        /// <summary>
        /// The parent model of this entity.
        /// </summary>
        public DataModel Parent { get; private set; }

        /// <summary>
        /// The name of this entity.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description about this entity.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The attributes of this entity.
        /// </summary>
        public IList<DataAttribute> Attributes { get { return attributes; } }

        /// <summary>
        /// The sources used to determine the definition of this entity.
        /// </summary>
        public IList<DataModelSource> Sources { get { return sources; } }


        public DataAttribute FindAttribute(string name)
        {
            return attributes.FirstOrDefault(x => x.Name == name);
        }


        public DataAttribute FindOrCreateAttribute(string name)
        {
            var attribute = attributes.FirstOrDefault(x => x.Name == name);

            if (attribute == null)
            {
                attribute = new DataAttribute(this, name);

                attributes.Add(attribute);
            }

            return attribute;
        }
    }
}
