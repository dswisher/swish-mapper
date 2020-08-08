
using System.Collections.Generic;

namespace SwishMapper.Models.Data
{
    public class MappedDataExpression
    {
        private readonly List<MappedDataArgument> arguments = new List<MappedDataArgument>();

        /// <summary>
        /// The function to be applied to the arguments. May be null.
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// The arguments to the function.
        /// </summary>
        /// <remarks>
        /// An expression can be just an identifier. If so, the FunctionName will be null, and there
        /// will be a single argument.
        /// </remarks>
        public List<MappedDataArgument> Arguments { get { return arguments; } }

        // TODO - implement ToString()
    }
}
