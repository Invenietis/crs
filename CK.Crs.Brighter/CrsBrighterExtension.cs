using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CK.Crs
{
    public static class CrsBrighterExtension
    {
        public static ICrsCoreBuilder AddBrighter(this ICrsCoreBuilder builder, Func<INeedPolicy, INeedARequestContext> configuration)
        {
            var partialBuilder = CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration(
                    subscriberRegistry: new RegistryAdapter( builder.Registry ),
                    asyncHandlerFactory: new BrighterCommandFactory( builder.Services.BuildServiceProvider() ) ) );

            var processorBuilder = configuration(partialBuilder).RequestContextFactory(new InMemoryRequestContextFactory());
            var processor = processorBuilder.Build();
            builder.Services.AddSingleton<IAmACommandProcessor>(processor);
            builder.Services.AddSingleton<ICommandDispatcher, BrighterCommandDispatcher>();

            return builder;
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
                return _registry.Registration.Where(t => t.CommandType == typeof(T)).Select(t => t.HandlerType);
            }

            void IAmASubscriberRegistry.Register<TRequest, TImplementation>()
            {
                _registry.Register( new CommandDescription(typeof(TRequest)) { HandlerType = typeof(TImplementation) } );
            }
        }
    }
}
