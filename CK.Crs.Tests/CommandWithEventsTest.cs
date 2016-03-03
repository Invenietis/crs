using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using CK.Core;
using CK.Crs.Runtime;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;

namespace CK.Crs.Tests
{
    public class CommandWithEventsTest
    {
        class SimpleCommand { }
        class SimpleEvent { }

        ITestOutputHelper _output;
        public CommandWithEventsTest( ITestOutputHelper output )
        {
            _output = output;
        }

        [Fact]
        public void an_event_emitter_should_be_set_when_events_are_published()
        {
            SimpleCommand command = new SimpleCommand();
            CommandExecutionContext ctx = new CommandExecutionContext(
                ( cctx ) => TestHelper.MockEventPublisher(),
                ( cctx ) => TestHelper.MockCommandScheduler(),
                TestHelper.Monitor(_output.WriteLine ),
                command,
                Guid.NewGuid(),
                false,
                String.Empty,
                default( CancellationToken));
            try
            {
                ctx.ExternalEvents.Push( new SimpleEvent() );
            }
            catch( Exception ex )
            {
                Assert.That( ex.GetBaseException(), Is.InstanceOf<InvalidOperationException>() );
            }
        }

        [Fact]
        public void when_an_event_emitter_is_configured_it_is_used_for_every_published_events()
        {
            SimpleCommand command = new SimpleCommand();
            CommandExecutionContext ctx = CreateContext( command );

            ManualResetEvent evt = new ManualResetEvent( false);
            bool eventEmitted = false;
            TransactionnalEventPublisher.SetEventEmitter( TestOperationExecutor.FromLambda( e =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject( e );
                _output.WriteLine( json );
                eventEmitted = true;
                evt.Set();
                return Task.FromResult<object>( null );
            } ) );

            Assert.That( eventEmitted, Is.False );
            ctx.ExternalEvents.Push( new SimpleEvent() );
            evt.WaitOne( 1000 );
            Assert.That( eventEmitted, Is.True );
        }


        [Fact]
        public void during_an_ambient_transaction_events_are_commited_when_the_transaction_is_commited()
        {
            SimpleCommand command = new SimpleCommand();
            CommandExecutionContext ctx = CreateContext( command );

            ManualResetEvent evt = new ManualResetEvent( false);
            int eventEmitted = 0;
            TransactionnalEventPublisher.SetEventEmitter( TestOperationExecutor.FromLambda( e =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject( e );
                _output.WriteLine( json );
                eventEmitted++;
                if( eventEmitted == 3 )
                {
                    evt.Set();
                }
                return Task.FromResult<object>( null );
            } ) );

            using( var tr = CreateTransactionScope() )
            {
                ctx.ExternalEvents.Push( new SimpleEvent() );
                ctx.ExternalEvents.Push( new SimpleEvent() );
                ctx.ExternalEvents.Push( new SimpleEvent() );
                Assert.That( eventEmitted, Is.EqualTo( 0 ) );
                tr.Complete();
            }

            evt.WaitOne( 1000 );
            Assert.That( eventEmitted, Is.EqualTo( 3 ) );
        }

        [Fact]
        public void during_an_ambient_transaction_events_are_NOT_commited_when_the_transaction_failed()
        {
            SimpleCommand command = new SimpleCommand();
            CommandExecutionContext ctx = CreateContext( command );
            ManualResetEvent evt = new ManualResetEvent( false);
            int eventEmitted = 0;
            TransactionnalEventPublisher.SetEventEmitter( TestOperationExecutor.FromLambda( e =>
            {
                eventEmitted++;
                return Task.FromResult<object>( null );
            } ) );

            try
            {
                using( var tr = CreateTransactionScope() )
                {
                    ctx.ExternalEvents.Push( new SimpleEvent() );
                    ctx.ExternalEvents.Push( new SimpleEvent() );
                    ctx.ExternalEvents.Push( new SimpleEvent() );

                    throw new Exception( "OUPS" );
                }
            }
            catch( Exception ex ) when( ex.Message == "OUPS" )
            {
            }
            
            evt.WaitOne( 500 );
            Assert.That( eventEmitted, Is.EqualTo( 0 ) );

            ctx.ExternalEvents.Push( new SimpleEvent() );

            evt.WaitOne( 1000 );
            Assert.That( eventEmitted, Is.EqualTo( 1 ), "Only one event Should be pushed, uncomitted failed events should have been cleared" );
        }

        [Fact]
        public void during_an_ambient_transaction_when_event_publishing_failed_the_whole_transaction_is_rollbacked()
        {
            SimpleCommand command = new SimpleCommand();
            CommandExecutionContext ctx = CreateContext( command );

            TransactionnalEventPublisher.SetEventEmitter( TestOperationExecutor.FromLambda( e =>
            {
                throw new InvalidOperationException( "OUPS EventEmitter is totally aux fraises..." );
            } ) );

            Assert.That( () =>
            {
                using( var tr = CreateTransactionScope() )
                {
                    ctx.ExternalEvents.Push( new SimpleEvent() );
                    ctx.ExternalEvents.Push( new SimpleEvent() );
                    ctx.ExternalEvents.Push( new SimpleEvent() );

                    // Other stuff transactionned... 

                    tr.Complete();
                }
            }, NUnit.Framework.Throws.Exception.InstanceOf<TransactionAbortedException>() );
        }

        TransactionScope CreateTransactionScope( IsolationLevel level = IsolationLevel.ReadUncommitted )
        {
            return new TransactionScope(
               TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = level },
               TransactionScopeAsyncFlowOption.Enabled );
        }

        private CommandExecutionContext CreateContext( SimpleCommand command )
        {
            return new CommandExecutionContext(
                ( cctx ) => new TransactionnalEventPublisher( cctx ),
                ( cctx ) => TestHelper.MockCommandScheduler(),
                TestHelper.Monitor( _output.WriteLine ),
                command,
                Guid.NewGuid(),
                false,
                String.Empty,
                default( CancellationToken ) );
        }


        class TestOperationExecutor : IOperationExecutor<Event>
        {
            Func<Event, Task> _asyncOperationExecution;
            public TestOperationExecutor( Func<Event, Task> asyncOperationExecution )
            {
                _asyncOperationExecution = asyncOperationExecution;
            }
            public void Execute( IActivityMonitor monitor, Event operation )
            {
                _asyncOperationExecution( operation );
            }

            public static Func<IOperationExecutor<Event>> FromLambda( Func<Event, Task> asyncOperationExecution )
            {
                return () => new TestOperationExecutor( asyncOperationExecution );
            }
        }
    }
}
