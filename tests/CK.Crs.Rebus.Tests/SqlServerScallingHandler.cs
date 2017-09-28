//using CK.Core;
//using Moq;
//using Rebus.Config;
//using Rebus.Handlers;
//using Rebus.Routing.TypeBased;
//using System;
//using System.Threading.Tasks;
//using R = Rebus;
//using NUnit.Framework;
//using MessageTemplates.Core;
//using Rebus.Pipeline;
//using CK.Crs.Rebus.Tests.Messages;

//namespace CK.Crs.Rebus.Tests
//{
//    [TestFixture]
//    public partial class SqlServerScallingHandler
//    {
//        class RebusHandler : IHandleMessages<SimpleCommand>
//        {
//            public Task Handle( SimpleCommand message )
//            {
//                var monitor = MessageContext.Current.GetActivityMonitor();
//                monitor.Info( $"The time is {DateTime.UtcNow} for {message.ActorId }" );

//                return Task.CompletedTask;
//            }
//        }

        

//        sealed class TestMonitorClient : ActivityMonitorTextWriterClient
//        {
//            public TestMonitorClient() : base( TestContext.Out.Write, LogFilter.Debug )
//            {
//            }
//        }

//        [Test]
//        public async Task TestMethod()
//        {
//            var conString = @"Server=.\SQLSERVER2016;Database=RebusQueue;Integrated Security=SSPI";
//            ActivityMonitor.AutoConfiguration = m => m.Output.RegisterClient( new TestMonitorClient() );

//            await Task.Run( async () =>
//            {
//                var activator = new R.Activation.BuiltinHandlerActivator();
//                activator.Register( () => new RebusHandler() );

//                var config = Configure.With( activator )
//                     .Logging( l => l.Use( new GrandOutputRebusLoggerFactory() ) )
//                     .Transport( t => t.UseSqlServer( conString, "tMessages", "command_executor" ) )
//                     .Routing( r => r.TypeBased().MapAssemblyOf<SimpleCommand>( "command_executor" ) );

//                using( var bus = config.Start() )
//                {
//                    await Task.Delay( 1000 );
//                }
//            } );

//            await Task.Run( async () =>
//            {
//                var config = Configure.With( new R.Activation.BuiltinHandlerActivator() )
//                     .Logging( l => l.Use( new GrandOutputRebusLoggerFactory() ) )
//                     .Transport( t => t.UseSqlServer( conString, "tMessages", "command_executor" ) )
//                     .Subscriptions( s => s.StoreInSqlServer( conString, "tSubscriptions" ) )
//                     .Routing( r => r.TypeBased().MapAssemblyOf<SimpleCommand>( "command_executor" ) );

//                using( var bus = config.Start() )
//                {
//                    var monitor = new ActivityMonitor();
//                    var crsBus = new RebusCommandReceiver( bus, new DefaultCommandReceiver(  );
//                    var context = new CommandContext( Guid.NewGuid(), typeof( SimpleCommand ), monitor, "123456" );
//                    var command = new SimpleCommand { ActorId = 10 };
//                    await crsBus.PostAsync( command, context );
//                }
//            } );

//            await Task.Delay( 1000 );
//        }
//    }
//}
