using CK.Core;
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
    public class CommandModel
    {
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

        /// <summary>
        /// Creates a command description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="handlerType"></param>
        public CommandModel( Type type, CKTraitContext context )
        {
            CommandType = type ?? throw new ArgumentNullException( nameof( type ) );
            Name = type.FullName;
            Tags = context.EmptyTrait;
            ResultType = FindResultType( type ); 
        }

        public static Type FindResultType( Type type )
        {
            string commandResultInterfaceName = nameof( ICommand<object> ) + "`1";
            var commandInterfaces = type.GetInterfaces();
            if( commandInterfaces.Length > 0 )
            {
                var commandResultInterface = commandInterfaces.FirstOrDefault( f => f.IsGenericType && f.Name == commandResultInterfaceName );
                if( commandResultInterface  != null )
                {
                    return commandResultInterface.GetGenericArguments()[0];
                }
            }
            // Find Nested Result class
            var commandResultType = Assembly.GetAssembly( type ).GetType( type.FullName + "+Result", false, true );
            if( commandResultType != null ) return commandResultType;

            return null;
        }

        /// <summary>
        /// The name of the command
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="System.Type"/> of the command to process.
        /// </summary>
        public Type CommandType { get; }

        /// <summary>
        /// The <see cref="System.Type"/> of the command to process.
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// Gets the command result type
        /// </summary>
        public Type ResultType { get; }

        //public Type HandlerSignatureType { get; }

        /// <summary>
        /// Gets commands traits
        /// This gives a hint to CRS to pick the best command executor.
        /// </summary>
        public CKTrait Tags { get; set; }

        /// <summary>
        /// Gets or sets a description for this command. Can you any format like Markdown or HTML.
        /// </summary>
        public string Description { get; set; }
    }
}
