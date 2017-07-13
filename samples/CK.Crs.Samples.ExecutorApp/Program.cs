using CK.Crs.Samples.Handlers;
using CK.Crs.Samples.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Paramore.Brighter.ServiceActivator;
using Paramore.Brighter;
using Newtonsoft.Json;
using System.Collections.Generic;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;

namespace CK.Crs.Samples.ExecutorApp
{
    class Program
    {
        static void Main( string[] args )
        {
            Console.WriteLine( "Hello World!" );

        }
    }

    class Startup
    {
        public void ConfigureCrsReceiver( IServiceCollection services )
        {
            services
                .AddCrsCore( config => config
                    .AddCommands( r => r.Register<SuperCommand>().HandledBy<SuperHandler>() )
                    .AddReceivers( e => e.For( typeof( CrsAzureReceiver<> ) ).AcceptAll() ) )
                .AddBrighterDispatcher( c => c.DefaultPolicy().NoTaskQueues() )
                .AddRabbitMQReceiver();

        }
    }

    class BrighterSuperCommand : SuperCommand, ICommand
    {
        public Guid Id { get; set; }
    }

    static class AzureReceiverExtensions
    {
        public static ICrsCoreBuilder AddRabbitMQReceiver( this ICrsCoreDispatcherBuilder builder )
        {
            var connections = new List<Connection>
            {
                new Connection<BrighterSuperCommand>(
                    new ConnectionName("paramore.example.greeting"),
                    new ChannelName("greeting.event"),
                    timeoutInMilliseconds: 200),

            };
            var connection = new RmqMessagingGatewayConnection
            {
                Name = "local",
                AmpqUri = new AmqpUriSpecification( new Uri( "amqp://guest:guest@localhost:5672/%2f" ) ),
                Exchange = new Exchange( "paramore.brighter.exchange" )
            };
            var consumer = new RmqMessageConsumerFactory( connection );
            var producer = new RmqMessageProducerFactory( connection );
            var channel = new InputChannelFactory( consumer , producer );
            var dispatcher = builder.DispatcherBuilder.DefaultChannelFactory( channel ).Connections( connections ).Build();
            dispatcher.Receive();
            return builder;
        }
    }

    class CrsAzureReceiver<T> : ICrsReceiver<T> where T : class
    {
        ICommandSender _dispatcher;
        public CrsAzureReceiver( ICommandSender dispatcher )
        {
            _dispatcher = dispatcher;
        }

        //[InputQueue( "my-azure-input-queue" )]
        //[OutputQueue( "my-azure-output-queue" )]
        public async Task<Response> ReceiveCommand( T command, ICommandContext context )
        {
            var result = await _dispatcher.SendAsync( command, context );
            return new Response( ResponseType.Synchronous, context.CommandId )
            {
                Payload = result
            };
        }
    }
}