using System;
using System.Collections.Generic;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// The command execution context
    /// </summary>
    public class CommandExecutionContext : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandExecutionContext( IServiceProvider serviceProvider, CommandReflectionContext reflectionContext )
        {
            if( serviceProvider == null ) throw new ArgumentNullException( "serviceProvider" );
            if( reflectionContext == null ) throw new ArgumentNullException( "reflectionContext" );

            _serviceProvider = serviceProvider;
            ReflectionContext = reflectionContext;
            Items = new Dictionary<object, object>();
        }

        public object Result { get; set; }

        public CommandReflectionContext ReflectionContext { get; private set; }

        /// <summary>
        /// Gets a bag of data that can hold data during the command execution
        /// </summary>
        public IDictionary<object, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets if there is an <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Get service
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService( Type serviceType ) =>  _serviceProvider.GetService( serviceType );
    }
}