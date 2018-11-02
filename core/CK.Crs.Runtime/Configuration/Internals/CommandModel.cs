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
    class CommandModel : ICommandModel
    {
        /// <summary>
        /// Creates a command description
        /// </summary>
        /// <param name="type"></param>
        /// <param name="context"></param>
        /// <param name="binder"></param>
        public CommandModel( Type type, CKTraitContext context, ICommandBinder binder = null )
            : this( type, null, context, binder )
        {
        }

        /// <summary>
        /// Creates a command description
        /// </summary>
        /// <param name="type"></param>
        /// <param name="resultType"></param>
        /// <param name="context"></param>
        /// <param name="binder"></param>
        public CommandModel( Type type, Type resultType, CKTraitContext context, ICommandBinder binder = null )
        {
            CommandType = type ?? throw new ArgumentNullException( nameof( type ) );
            Name = new CommandName( type );
            Tags = context.EmptyTrait;
            ResultType = resultType ?? FindResultType( type );
        }


        public static Type FindResultType( Type type )
        {
            string commandResultInterfaceName = nameof( ICommand<object> ) + "`1";
            var commandInterfaces = type.GetInterfaces();
            if( commandInterfaces.Length > 0 )
            {
                var commandResultInterface = commandInterfaces.FirstOrDefault( f => f.IsGenericType && f.Name == commandResultInterfaceName );
                if( commandResultInterface != null )
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
        public CommandName Name { get; internal set; }

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

        /// <summary>
        /// Gets or sets the binder type for this command. If null, a default binder is applied.
        /// </summary>
        public ICommandBinder Binder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Type> Filters { get; set; }

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
