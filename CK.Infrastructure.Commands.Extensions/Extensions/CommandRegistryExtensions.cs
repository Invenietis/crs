using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{

    public static class CommandReceiverExtensions
    {
        public static CommandReceiverOptions Register( 
            this CommandReceiverOptions o, 
            Type commandType, 
            Type handlerType, 
            string route, 
            bool isLongRunning )
        {
            var desc = new CommandDescriptor
            {
                CommandType = commandType,
                HandlerType = handlerType,
                Route = route,
                IsLongRunning = isLongRunning
            };
            desc.Decorators = ExtractDecoratorsFromHandlerAttributes( desc.CommandType, desc.HandlerType )
                .Union( ApplyGlobalDecorators( o ), EqualityComparer<Type>.Default )
                .ToArray();

            o.Registry.Register( desc );

            return o;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="route"></param>
        /// <param name="isLongRunning"></param>
        /// <returns></returns>
        public static CommandReceiverOptions Register<TCommand, THandler>( this CommandReceiverOptions o, string route, bool isLongRunning )
            where TCommand : class
            where THandler : class, ICommandHandler<TCommand>
        {
            return o.Register( typeof( TCommand ), typeof( THandler ), route, isLongRunning );
        }

        private static IEnumerable<Type> ApplyGlobalDecorators( CommandReceiverOptions o )
        {
            if( _globalDecorators.IsValueCreated && _globalDecorators.Value.ContainsKey( o ) )
            {
                return _globalDecorators.Value[o];
            }
            return Enumerable.Empty<Type>();
        }

        private static IReadOnlyCollection<Type> ExtractDecoratorsFromHandlerAttributes( Type commandType, Type handlerType )
        {
            return handlerType.GetCustomAttributes( true ).OfType<ICommandDecorator>().Select( a => a.GetType() ).ToArray();
        }

        static Lazy<Dictionary<CommandReceiverOptions,List<Type>>> _globalDecorators = new Lazy<Dictionary<CommandReceiverOptions,List<Type>>>( () => new Dictionary<CommandReceiverOptions, List<Type>>());

        public static CommandReceiverOptions AddGlobalDecorator( this CommandReceiverOptions o, Type decoratorType )
        {
            if( !_globalDecorators.Value.ContainsKey( o ) ) _globalDecorators.Value.Add( o, new List<Type>() );
            _globalDecorators.Value[o].Add( decoratorType );
            return o;
        }

        public static CommandReceiverOptions AddGlobalDecorator<TDecorator>( this CommandReceiverOptions o ) where TDecorator : ICommandDecorator
        {
            return AddGlobalDecorator( o, typeof( TDecorator ) ); ;
        }
    }
}