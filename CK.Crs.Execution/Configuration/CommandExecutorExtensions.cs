using System;
using CK.Crs.Runtime.Execution;
using Paramore.Brighter;
using CK.Crs;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommandExecutorExtensions
    {
        public static void AddCommandExecutor( this IServiceCollection services, Action<ExecutorOption> configuration = null )
        {
            // The execution factory is scoped to the execution of the command.
            services.AddScoped<IExecutionFactory, DefaultExecutionFactory>();

            ExecutorOption config = new ExecutorOption();
            configuration?.Invoke( config );

            services.AddSingleton( typeof( ICommandRunningStore ), config.CommandRunningStore );

        }

        public static void AddBrighterExecutor( this IServiceCollection services, ICommandRegistry registry, Func<INeedPolicy, INeedARequestContext> configuration )
        {
            var factory = new DefaultExecutionFactory(services.BuildServiceProvider());

            var partialBuilder = CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration(
                    subscriberRegistry: new RegistryAdapter(registry),
                    handlerFactory: factory,
                    asyncHandlerFactory: factory));
            
            var builder = configuration(partialBuilder).RequestContextFactory(new InMemoryRequestContextFactory());
            var processor = builder.Build();
            services.AddSingleton<IAmACommandProcessor>(processor);
        }


        class RegistryAdapter : IAmASubscriberRegistry
        {
            ICommandRegistry _registry;
            public RegistryAdapter(ICommandRegistry registry)
            {
                _registry = registry;
            }
            IEnumerable<Type> IAmASubscriberRegistry.Get<T>()
            {
                return _registry.Registration.Where( t => t.CommandType == typeof(T) ).Select( t => t.HandlerType );
            }

            void IAmASubscriberRegistry.Register<TRequest, TImplementation>()
            {
                _registry.Register<TRequest>().HandledBy<TImplementation>();
            }
        }
    }
}
