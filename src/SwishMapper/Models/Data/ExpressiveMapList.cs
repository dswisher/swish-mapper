
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class ExpressiveMapList
    {
        private readonly List<ExpressiveMapping> maps = new List<ExpressiveMapping>();
        private readonly HashSet<string> ignoredAttributes = new HashSet<string>();

        public ExpressiveMapList(string filename)
        {
            FileName = filename;
        }

        public string FileName { get; private set; }
        public string Name { get; set; }
        public IList<ExpressiveMapping> Maps { get { return maps; } }


        public void AddIgnored(MappedDataAttribute attribute)
        {
            ignoredAttributes.Add(MakeKey(attribute.Attribute));
        }


        public bool IsIgnored(DataAttribute attribute)
        {
            return ignoredAttributes.Contains(MakeKey(attribute));
        }


        private string MakeKey(DataAttribute attribute)
        {
            return $"{attribute.Parent.Parent.Id}-{attribute.Parent.Name}-{attribute.Name}";
        }
    }
}
