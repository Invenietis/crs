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
            ICommandHandler<SimpleCommandWithResult.Result>,
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

            public Task HandleAsync( SimpleCommandWithResult.Result command, ICommandContext context )
            {
                var monitor = context.Monitor;
                monitor.Info( $"The time from the result is {command.Date} from {command.GetType().Name}" );
                return Task.CompletedTask;
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
                            .Register<SimpleCommandWithResult>().IsRebusQueue().HandledBy<SimpleCommandHandler>()
                            .Register<SimpleCommandWithResult.Result>().IsRebusQueue().HandledBy<SimpleCommandHandler>() ) )
                    .AddRebus(
                        c => c.Transport( t => t.UseInMemoryTransport( new InMemNetwork( true ), "commands_result" ) ),
                        c => c( "commands" )( r => r.HasRebusQueueTag() && r.HandlerType == null ),
                        c => c( "commands_result" )( r => r.HasRebusQueueTag() && r.HandlerType != null ) );

                using( var services = serviceCollection.BuildServiceProvider() )
                {
                    var monitor = new ActivityMonitor(); // Request context
                    var dispatcher = services.GetRequiredService<ICommandReceiver>();


                    var context = EnsureContext<SimpleCommand>( services, monitor );
                    var response = await dispatcher.ReceiveCommand( new SimpleCommand { ActorId = 421 }, context );
                    Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Synchronous ) );
                    Assert.That( response.Payload, Is.Null );

                    context = EnsureContext<SimpleCommandWithDirectResult>( services, monitor );
                    response = await dispatcher.ReceiveCommand( new SimpleCommandWithDirectResult { ActorId = 421 }, context );
                    Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Synchronous ) );
                    Assert.That( response.Payload, Is.Not.Null.And.AssignableFrom<DateTime>() );

                    context = EnsureContext<SimpleCommandWithResult>( services, monitor );
                    response = await dispatcher.ReceiveCommand( new SimpleCommandWithResult { ActorId = 421 }, context );
                    Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Asynchronous ) );
                    Assert.That( response.Payload, Is.Null );

                    await Task.Delay( 500 );
                }
            }
        }


        [Test]
        public async Task Test_DifferentProcess()
        {
            using( GrandOutput.EnsureActiveDefault( new GrandOutputConfiguration
            {
                Handlers = { new TextWriterHandlerConfiguration { Out = TestContext.Out } }
            } ) )
            {
                var sharedNetwork = new InMemNetwork( true );
                sharedNetwork.CreateQueue( "commands" );
                sharedNetwork.CreateQueue( "commands_result" );

                // CONSUMER OF COMMANDS AND PRODUCER OF RESULTS
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                Task.Run( async () =>
                {
                    var serviceCollection = new ServiceCollection();
                    serviceCollection
                    .AddCrsCore(
                        c => c.Commands( registry => registry
                            .Register<SimpleCommand>().HandledBy<SimpleCommandHandler>()
                            .Register<SimpleCommandWithDirectResult>().HandledBy<SimpleCommandHandler>()
                            .Register<SimpleCommandWithResult>().HandledBy<SimpleCommandHandler>()
                            .Register<SimpleCommandWithResult.Result>().IsRebusQueue() ) )
                    .AddRebus(
                        c => c.Transport( t => t.UseInMemoryTransport( sharedNetwork, "commands" ) ),
                        c => c( "commands" )( r => r.HandlerType != null ),
                        c => c( "commands_result" )( r => r.HasRebusQueueTag() ) );

                    using( var services = serviceCollection.BuildServiceProvider() )
                    {
                        // Wait for incoming commands...
                        await Task.Delay( 800 );
                    }
                } );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                // PRODUCER OF COMMANDS AND CONSUMER OF RESULTS
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run( async () =>
                {
                    var serviceCollection = new ServiceCollection();
                    serviceCollection
                        .AddCrsCore(
                            c => c.Commands( registry => registry
                                .Register<SimpleCommand>().IsRebusQueue()
                                .Register<SimpleCommandWithDirectResult>().IsRebusQueue()
                                .Register<SimpleCommandWithResult>().IsRebusQueue()
                                .Register<SimpleCommandWithResult.Result>().IsRebusQueue().HandledBy<SimpleCommandHandler>() ) )
                        .AddRebus(
                            c => c.Transport( t => t.UseInMemoryTransport( sharedNetwork, "commands_result" ) ),
                            c => c( "commands" )( r => r.HasRebusQueueTag() && r.HandlerType == null ),
                            c => c( "commands_result" )( r => r.HasRebusQueueTag() && r.HandlerType != null ) );

                    using( var services = serviceCollection.BuildServiceProvider() )
                    {
                        var monitor = new ActivityMonitor(); // Request context
                        var dispatcher = services.GetRequiredService<ICommandReceiver>();

                        var context = EnsureContext<SimpleCommand>( services, monitor );
                        var response = await dispatcher.ReceiveCommand( new SimpleCommand { ActorId = 421 }, context );
                        Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Asynchronous ) );

                        context = EnsureContext<SimpleCommandWithDirectResult>( services, monitor );
                        response = await dispatcher.ReceiveCommand( new SimpleCommandWithDirectResult { ActorId = 421 }, context );
                        Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Asynchronous ) );

                        context = EnsureContext<SimpleCommandWithResult>( services, monitor );
                        response = await dispatcher.ReceiveCommand( new SimpleCommandWithResult { ActorId = 421 }, context );
                        Assert.That( response.ResponseType, Is.EqualTo( (char)ResponseType.Asynchronous ) );

                        // Wait for incoming results...
                        await Task.Delay( 800 );
                    }
                } );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                await Task.Delay( 2000 );
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
