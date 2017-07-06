using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class DefaultCommandDispatcher : ICommandDispatcher
    {
        readonly IServiceProvider _services;
        readonly ICommandRegistry _registry;

        public DefaultCommandDispatcher( IServiceProvider services, ICommandRegistry registry )
        {
            _services = services;
            _registry = registry;
        }

        public Task SendAsync<T>(T command, ICommandExecutionContext context) where T : class
        {
            var desc = _registry.Registration.SingleOrDefault( c => c.CommandType == typeof(T) );
            if (desc == null) throw new ArgumentException( String.Format( "Command {0} not registered", typeof(T).Name) );

            var handler = CreateInstanceOrDefault<ICommandHandler<T>>( context, desc.HandlerType );
            if (handler == null) throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to created", desc.HandlerType) );
            try
            {
                return handler.HandleAsync(context, command);
            }
            catch( Exception ex )
            {
                context.Monitor.Error().Send(ex);
                return Task.CompletedTask;
            }
        }

        public T CreateInstanceOrDefault<T>( ICommandExecutionContext context, Type instanceType, Func<T> defaultActivator = null) where T : class
        {
            if (!typeof(T).IsAssignableFrom( instanceType )) 
                throw new InvalidOperationException($"{typeof(T)} is not assignable from {instanceType}");
            try
            {
                T inst = _services.GetService(instanceType) as T;
                return inst ?? (defaultActivator != null ? defaultActivator() : ActivatorUtilities.CreateInstance(_services, instanceType) as T);
            }
            catch (Exception ex)
            {
                context.Monitor.Error().Send(ex);
                return null;
            }
        }
    }
}
