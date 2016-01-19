using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{

    public static class CommandReceiverExtensions
    {
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
            var desc = new CommandDescriptor
            {
                CommandType = typeof( TCommand ),
                HandlerType = typeof( THandler ),
                Route = route,
                IsLongRunning = isLongRunning
            };
            desc.Decorators = ExtractDecoratorsFromHandlerAttributes( desc.CommandType, desc.HandlerType )
                .Union( ApplyGlobalDecorators() )
                .ToArray();

            o.Registry.Register( desc );

            return o;
        }

        private static IEnumerable<Type> ApplyGlobalDecorators()
        {
            if( _globalDecorators.IsValueCreated )
            {
                return _globalDecorators.Value;
            }
            return Enumerable.Empty<Type>();
        }

        private static IReadOnlyCollection<Type> ExtractDecoratorsFromHandlerAttributes( Type commandType, Type handlerType )
        {
            var q = (from m in handlerType.GetMethods( System.Reflection.BindingFlags.FlattenHierarchy  )
                     let commandTypeParameter = m.GetParameters()[0].ParameterType.GetGenericArguments()[0]
                     where (m.Name == "DoHandleAsync" && commandType.IsAssignableFrom( commandTypeParameter ) )
                     select new
                     {
                         Attributes = m.GetCustomAttributes( true ).OfType<HandlerAttributeBase>(),
                         HandleMethod = m
                     }).FirstOrDefault();

            return q.Attributes.Select( a => a.GetType() ).ToArray();
        }

        static Lazy<List<Type>> _globalDecorators = new Lazy<List<Type>>( () => new List<Type>());

        public static CommandReceiverOptions AddGlobalDecorator<TDecorator>( this CommandReceiverOptions o ) where TDecorator : ICommandDecorator
        {
            _globalDecorators.Value.Add( typeof( TDecorator ) );
            return o;
        }
    }
}