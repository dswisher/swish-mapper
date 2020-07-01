
using System.Collections.Generic;

namespace SwishMapper.Formatters
{
    public class DocRoot : DocNode
    {
        private readonly List<DocNode> children = new List<DocNode>();

        public string Title { get; set; }
        public string Style { get; set; }

        public IList<DocNode> Children { get { return children; } }
    }
}
