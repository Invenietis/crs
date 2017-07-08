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
    public class DefaultBus : IBus
    {
        readonly IServiceProvider _services;
        readonly IRequestRegistry _registry;

        public DefaultBus( IServiceProvider services, IRequestRegistry registry )
        {
            _services = services;
            _registry = registry;
        }

        public async Task PublishAsync<T>(T evt, ICommandContext context) where T : class
        {
            var all = _registry.Registration.Where( c => c.Type == typeof(T) );
            foreach( var desc in all) await DoSendAsync(evt, context, desc);
        }

        public Task SendAsync<T>(T command, ICommandContext context) where T : class
        {
            var desc = _registry.Registration.SingleOrDefault(c => c.Type == typeof(T));
            return DoSendAsync(command, context, desc);
        }

        private Task DoSendAsync<T>(T command, ICommandContext context, RequestDescription desc) where T : class
        {
            if (desc == null) throw new ArgumentException(String.Format("Command {0} not registered", typeof(T).Name));

            var handler = CreateInstanceOrDefault<IRequestHandler<T>>(context, desc.HandlerType);
            if (handler == null) throw new ArgumentException(String.Format("Handler {0} for {1} impossible to created", desc.HandlerType));
            try
            {
                return handler.HandleAsync( command, context );
            }
            catch (Exception ex)
            {
                context.Monitor.Error().Send(ex);
                return Task.CompletedTask;
            }
        }

        public T CreateInstanceOrDefault<T>( ICommandContext context, Type instanceType, Func<T> defaultActivator = null) where T : class
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
