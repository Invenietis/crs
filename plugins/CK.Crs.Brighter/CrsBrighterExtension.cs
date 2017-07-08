using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    public static class CrsBrighterExtension
    {
        public static ICrsCoreBuilder AddBrighter(this ICrsCoreBuilder builder, Func<INeedPolicy, INeedARequestContext> configuration)
        {
            var partialBuilder = CommandProcessorBuilder.With()
                .Handlers(new HandlerConfiguration(
                    subscriberRegistry: new RegistryAdapter( builder.Registry ),
                    asyncHandlerFactory: new BrighterCommandFactory( new Lazy<IServiceProvider>( () => builder.Services.BuildServiceProvider() ) ) ) );

            var processorBuilder = configuration(partialBuilder).RequestContextFactory(new InMemoryRequestContextFactory());
            var processor = processorBuilder.Build();
            builder.Services.AddSingleton<IAmACommandProcessor>(processor);
            builder.Services.AddSingleton<IBus, BrighterCommandDispatcher>();

            return builder;
        }

        class BrighterGenericHandler<T> : RequestHandlerAsync<T> where T : class, IRequest
        {
            public override Task<T> HandleAsync(T command, CancellationToken cancellationToken = default(CancellationToken))
            {
                return base.HandleAsync(command, cancellationToken);
            }
        }

        class RegistryAdapter : IAmASubscriberRegistry
        {
            IRequestRegistry _registry;
            public RegistryAdapter(IRequestRegistry registry)
            {
                _registry = registry;
            }
            IEnumerable<Type> IAmASubscriberRegistry.Get<T>()
            {
                return _registry.Registration.Where(t => t.Type == typeof(T)).Select(t => t.HandlerType);
            }

            void IAmASubscriberRegistry.Register<TRequest, TImplementation>()
            {
                _registry.Register( new RequestDescription(typeof(TRequest)) { HandlerType = typeof(TImplementation) } );
            }
        }
    }
}
