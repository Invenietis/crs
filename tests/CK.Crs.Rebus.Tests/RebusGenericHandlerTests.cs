using NUnit.Framework;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rebus.Handlers;
using Rebus.Transport;
using System.Linq;
using Moq;
using CK.Crs.Rebus.Tests.Messages;
using CK.Core;
using Rebus.Transport.InMem;
using CK.Monitoring;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Rebus.Tests
{
    [TestFixture]
    public class RebusGenericHandlerTests
    {


        class SimpleCommandHandler :
            ICommandHandler<SimpleCommand>,
            ICommandHandler<SimpleCommandWithResult, SimpleCommandWithResult.Result>,
            ICommandHandler<SimpleCommandWithDirectResult, DateTime>
        {
            public Task HandleAsync( SimpleCommand command, ICommandContext context )
            {
                var monitor = context.Monitor;
                Assert.That( monitor, Is.Not.Null );

                monitor.Info( $"The time is {DateTime.UtcNow} for {command.ActorId } from {command.GetType().Name}" );

                return Task.CompletedTask;
            }

            public Task<SimpleCommandWithResult.Result> HandleAsync( SimpleCommandWithResult command, ICommandContext context )
            {
                var monitor = context.Monitor;
                Assert.That( monitor, Is.Not.Null );

                monitor.Info( $"The time is {DateTime.UtcNow} for {command.ActorId } from {command.GetType().Name}" );

                return Task.FromResult( new SimpleCommandWithResult.Result { Date = DateTime.UtcNow } );
            }

            public Task<DateTime> HandleAsync( SimpleCommandWithDirectResult command, ICommandContext context )
            {
                var monitor = context.Monitor;
                Assert.That( monitor, Is.Not.Null );

                monitor.Info( $"The time is {DateTime.UtcNow} for {command.ActorId } from {command.GetType().Name}" );

                return Task.FromResult( DateTime.UtcNow );
            }
        }


        [Test]
        public async Task Test()
        {
            using( GrandOutput.EnsureActiveDefault( new GrandOutputConfiguration
            {
                Handlers = { new TextWriterHandlerConfiguration { Out = TestContext.Out } }
            } ) )
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection
                    .AddCrsCore(
                        c => c.Commands( registry => registry
                            .Register<SimpleCommand>().HandledBy<SimpleCommandHandler>() 
                            .Register<SimpleCommandWithDirectResult>().HandledBy<SimpleCommandHandler>() 
                            .Register<SimpleCommandWithResult>().HandledBy<SimpleCommandHandler>() ) )
                    .AddRebus( c => c
                        .Transport( t => t.UseInMemoryTransport( new InMemNetwork( true ), "command_executor" ) )
                        .Routing( r => r.TypeBased().MapAssemblyOf<SimpleCommand>( "command_executor" ) ) );

                using( var services = serviceCollection.BuildServiceProvider() )
                {

                    var dispatcher = services.GetRequiredService<ICommandReceiver>();

                    // Request context
                    var monitor = new ActivityMonitor();
                    var context = EnsureContext<SimpleCommand>( services, monitor );

                    var response = await dispatcher.ReceiveCommand( new SimpleCommand { ActorId = 421 }, context );
                    Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Synchronous ) );
                    Assert.That( response.Payload, Is.Null );

                    context = EnsureContext<SimpleCommandWithResult>( services, monitor );
                    response = await dispatcher.ReceiveCommand( new SimpleCommandWithResult { ActorId = 421 }, context );
                    Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Synchronous ) );
                    Assert.That( response.Payload, Is.Not.Null.And.AssignableFrom<SimpleCommandWithResult.Result>() );


                    context = EnsureContext<SimpleCommandWithDirectResult>( services, monitor );
                    response = await dispatcher.ReceiveCommand( new SimpleCommandWithDirectResult { ActorId = 421 }, context );
                    Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Synchronous ) );
                    Assert.That( response.Payload, Is.Not.Null.And.AssignableFrom<DateTime>() );

                    await Task.Delay( 500 );

                }
            }
        }

        private CommandContext EnsureContext<T>( IServiceProvider services, IActivityMonitor monitor )
        {
            return new CommandContext(
                Guid.NewGuid(),
                monitor,
                services.GetRequiredService<ICommandRegistry>().Registration.SingleOrDefault( r => r.CommandType == typeof( T ) ),
                "123456" );
        }
    }
}