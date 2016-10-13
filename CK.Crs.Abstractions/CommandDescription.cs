using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Describes a command and its environment.
    /// </summary>
    public class CommandDescription
    {
        public CommandDescription()
        {
            Decorators = CK.Core.Util.Array.Empty<Type>();
        }

        /// <summary>
        /// The name of the command
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the command to process.
        /// </summary>
        public Type CommandType { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the command to process.
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// Gets commands traits
        /// This gives a hint to CRS to pick the best command executor.
        /// </summary>
        public string Traits { get; set; }

        /// <summary>
        /// Gets or sets a description for this command. Can you any format like Markdown or HTML.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a read-only collection of decorators that should be applied when this command is handled.
        /// A decorator encapsulate common behaviors or cross-cutting-concerns accross commands of a system, like authorization, logging, transaction etc.
        /// </summary>
        public IReadOnlyCollection<Type> Decorators { get; set; }

        IDictionary<string, object> _extraData;
        /// <summary>
        /// Gets a dictionary of extra data.
        /// </summary>
        public IDictionary<string, object> ExtraData
        {
            get { return _extraData ?? (_extraData = new Dictionary<string, object>()); }
        }
    }
}
