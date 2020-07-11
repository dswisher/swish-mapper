
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

        /// <summary>
        /// The name of this entity.
        /// </summary>
        public string Name { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// The attributes of this entity.
        /// </summary>
        public IList<DataAttribute> Attributes { get { return attributes; } }


        public DataAttribute FindOrCreateAttribute(string name)
        {
            var attribute = attributes.FirstOrDefault(x => x.Name == name);

            if (attribute == null)
            {
                attribute = new DataAttribute
                {
                    Name = name
                };

                attributes.Add(attribute);
            }

            return attribute;
        }
    }
}
