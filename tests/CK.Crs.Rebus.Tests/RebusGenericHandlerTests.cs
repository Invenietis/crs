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


        class SimpleCommandHandler : ICommandHandler<SimpleCommand>
        {
            public Task HandleAsync( SimpleCommand command, ICommandContext context )
            {
                var monitor = context.Monitor;
                Assert.That( monitor, Is.Not.Null );

                monitor.Info( $"The time is {DateTime.UtcNow} for {command.ActorId }" );

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
                    .AddCrsCore( c => c.AddCommands( r => r.Register<SimpleCommand>().HandledBy<SimpleCommandHandler>() ) )
                    .AddRebus( c => c
                        .Transport( t => t.UseInMemoryTransport( new InMemNetwork( true ), "command_executor" ) )
                        .Routing( r => r.TypeBased().MapAssemblyOf<SimpleCommand>( "command_executor" ) ) );

                var services = serviceCollection.BuildServiceProvider();

                var bus = services.GetRequiredService<IBus>();

                // Request context
                var monitor = new ActivityMonitor();
                var context = new CommandContext( Guid.NewGuid(), typeof( SimpleCommand ), monitor, "123456" );

                await bus.SendAsync( new SimpleCommand { ActorId = 421 }, context );

                await Task.Delay( 500 );

                ((IDisposable)bus)?.Dispose();
            }
        }
    }
}
