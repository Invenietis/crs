using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Paramore.Brighter;
using Paramore.Brighter.ServiceActivator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{

    public interface ICrsCoreDispatcherBuilder : ICrsCoreBuilder
    {
        INeedAChannelFactory DispatcherBuilder { get; }
    }

    public class CrsCoreDispatcherBuilder : ICrsCoreDispatcherBuilder
    {
        readonly ICrsCoreBuilder _builder;

        public CrsCoreDispatcherBuilder( INeedAChannelFactory dispatcherBuilder, ICrsCoreBuilder builder ) 
        {
            DispatcherBuilder = dispatcherBuilder;
        }

        public INeedAChannelFactory DispatcherBuilder { get; }

        public IServiceCollection Services => _builder.Services;

        public ICommandRegistry Registry => _builder.Registry;

        public ICrsModel Model => _builder.Model;
    }

    public static class CrsBrighterExtension
    {
        public static ICrsCoreBuilder AddBrighter(this ICrsCoreBuilder builder, Func<INeedPolicy, INeedARequestContext> configuration)
        {
            CommandProcessor processor = BuildCommandProcessor( builder, configuration );
            builder.Services.AddSingleton<IAmACommandProcessor>( processor );
            builder.Services.AddSingleton<IBus, BrighterCommandDispatcher>();

            return builder;
        }

        public static ICrsCoreDispatcherBuilder AddBrighterDispatcher( 
            this ICrsCoreBuilder builder, 
            Func<INeedPolicy, INeedARequestContext> configuration )
        {
            CommandProcessor processor = BuildCommandProcessor( builder, configuration );
            builder.Services.AddSingleton<IAmACommandProcessor>( processor );

            var messageMapperFactory = new MessageMapperFactory();
            var messageMapperRegistry = new MessageMapperRegistry( messageMapperFactory );
            // Autoregister mappers

            var partialBuilder = DispatchBuilder.With()
                .CommandProcessor( processor )
                .MessageMappers( messageMapperRegistry );

            return new CrsCoreDispatcherBuilder( partialBuilder, builder );
        }

        private static CommandProcessor BuildCommandProcessor( ICrsCoreBuilder builder, Func<INeedPolicy, INeedARequestContext> configuration )
        {
            var partialBuilder = CommandProcessorBuilder.With()
                .Handlers( new HandlerConfiguration(
                    subscriberRegistry: new RegistryAdapter( builder.Registry ),
                    asyncHandlerFactory: new BrighterCommandFactory( new Lazy<IServiceProvider>( () => builder.Services.BuildServiceProvider() ) ) ) );

            var processorBuilder = configuration( partialBuilder ).RequestContextFactory( new InMemoryRequestContextFactory() );
            var processor = processorBuilder.Build();
            return processor;
        }

        class BrighterGenericHandler<T> : RequestHandlerAsync<T> where T : class, IRequest
        {
            public override Task<T> HandleAsync(T command, CancellationToken cancellationToken = default(CancellationToken))
            {
                return base.HandleAsync(command, cancellationToken);
            }
        }

        public class GenericCommandMapper<T> : IAmAMessageMapper<T> where T : class, ICommand
        {
            public Message MapToMessage( T request )
            {
                var header = new MessageHeader( messageId: request.Id, topic: request.GetType().Name, messageType: MessageType.MT_COMMAND );
                var body = new MessageBody( JsonConvert.SerializeObject( request ) );
                var message = new Message( header, body );
                return message;
            }

            public T MapToRequest( Message message )
            {
                return JsonConvert.DeserializeObject<T>( message.Body.Value );
            }
        }
        class MessageMapperFactory : IAmAMessageMapperFactory
        {
            public IAmAMessageMapper Create( Type messageMapperType )
            {
                return (IAmAMessageMapper)Activator.CreateInstance( messageMapperType );
            }
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
                _registry.Register( new CommandModel(typeof(TRequest)) { HandlerType = typeof(TImplementation) } );
            }
        }
    }
}
