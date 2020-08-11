
using System.Collections.Generic;
using System.Linq;

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


        public override string ToString()
        {
            var args = string.Join(", ", Arguments.Select(x => x.ToString()));
            if (FunctionName != null)
            {
                return $"{FunctionName}({args})";
            }
            else
            {
                return args;
            }
        }
    }
}
