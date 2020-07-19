
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
        private readonly List<DataEntity> referencedBy = new List<DataEntity>();
        private readonly List<DataAttributeSample> outcastSamples = new List<DataAttributeSample>();


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

        /// <summary>
        /// Other entities that reference this entity.
        /// </summary>
        public IList<DataEntity> ReferencedBy { get { return referencedBy; } }

        /// <summary>
        /// Samples that do not have a home, as their paths do match the model in some way.
        /// </summary>
        public List<DataAttributeSample> OutcastSamples { get { return outcastSamples; } }

        public bool HasSamples
        {
            get
            {
                return attributes.Any(x => x.Samples.Count > 0);
            }
        }


        public bool HasEnums
        {
            get
            {
                return attributes.Any(x => x.EnumValues.Count > 0);
            }
        }


        public DataAttribute FindAttribute(string name)
        {
            return attributes.FirstOrDefault(x => x.Name == name);
        }


        public DataAttribute FindOrCreateAttribute(string name, DataModelSource source)
        {
            return FindOrCreateAttribute(name, new[] { source });
        }


        public DataAttribute FindOrCreateAttribute(string name, IEnumerable<DataModelSource> sources)
        {
            var attribute = attributes.FirstOrDefault(x => x.Name == name);

            if (attribute == null)
            {
                attribute = new DataAttribute(this, name);

                attributes.Add(attribute);
            }

            foreach (var source in sources)
            {
                if (!attribute.Sources.Any(x => x.ShortName == source.ShortName))
                {
                    attribute.Sources.Add(source);
                }
            }

            return attribute;
        }
    }
}
