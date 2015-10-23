using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandHandlerRegistry : ICommandHandlerRegistry
    {
        Dictionary<Type, Type> HandlerMap { get; } = new Dictionary<Type, Type>();

        public Type GetHandlerType( Type commandType )
        {
            Type t;
            HandlerMap.TryGetValue( commandType, out t );
            return t;
        }

        public bool IsRegisterd( Type commandType )
        {
            return HandlerMap.ContainsKey( commandType );
        }

        public void RegisterHandler<T, THandler>() where THandler : ICommandHandler
        {
            if( HandlerMap.ContainsKey( typeof( T ) ) )
                throw new ArgumentException( "An handler is already registered for this command " + typeof( T ).FullName );

            HandlerMap.Add( typeof( T ), typeof( THandler ) );
        }

    }
}
