
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class MappedDataExpression
    {
        private readonly List<MappedDataAttribute> arguments = new List<MappedDataAttribute>();

        /// <summary>
        /// The function to be applied to the arguments. May be null.
        /// </summary>
        public string FunctionName { get; set; }

        // TODO - xyzzy - FIXME - arguments can be: 1) an attribute, 2) a model name, 3) a string; how to support 2 & 3???

        /// <summary>
        /// The arguments to the function.
        /// </summary>
        /// <remarks>
        /// An expression can be just an identifier. If so, the FunctionName will be null, and there
        /// will be a single argument.
        /// </remarks>
        public List<MappedDataAttribute> Arguments { get { return arguments; } }
    }
}
