using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Describes a command and its environment.
    /// </summary>
    public class CommandDescription
    {
        public static string GetDefaultName(Type type) => RemoveSuffixes( type, "Command", "Cmd");

        internal static string RemoveSuffixes( Type t, params string[] suffixes)
        {
            var s = t.Name;
            foreach (var suf in suffixes)
            {
                if (s.EndsWith(suf, StringComparison.OrdinalIgnoreCase))
                {
                    int idx = s.IndexOf(suf);
                    return s.Substring(0, idx);
                }
            }
            return s;
        }
        public static IReadOnlyCollection<Type> ExtractDecoratorsFromHandlerAttributes(Type commandType, Type handlerType) =>
            handlerType.GetTypeInfo().GetCustomAttributes().OfType<ICommandDecorator>().Select(t => t.GetType()).ToArray();

        /// <summary>
        /// Creates a command description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="commandType"></param>
        /// <param name="handlerType"></param>
        public CommandDescription( Type commandType )
        {
            CommandType = commandType ?? throw new ArgumentNullException( nameof( commandType ) );
            Name = GetDefaultName( commandType );
            Decorators = CK.Core.Util.Array.Empty<Type>();
            Traits = String.Empty;
        }

        /// <summary>
        /// The name of the command
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="Type"/> of the command to process.
        /// </summary>
        public Type CommandType { get; }

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
