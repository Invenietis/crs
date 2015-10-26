using System;
using System.Collections.Generic;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// The command execution context
    /// </summary>
    public class CommandExecutionContext
    {

        public CommandExecutionContext( CommandContext commandContext )
        {
            if( commandContext == null ) throw new ArgumentNullException( "commandContext" );

            RuntimeContext = commandContext;
            Items = new Dictionary<object, object>();
        }

        public CommandContext RuntimeContext { get; private set; }

        /// <summary>
        /// Gets a bag of data that can hold data during the command execution
        /// </summary>
        public IDictionary<object, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the result of the command.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets if there is an <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; set; }
    }
}