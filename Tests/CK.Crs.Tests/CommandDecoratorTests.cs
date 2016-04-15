using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using NUnit.Framework;
using CK.Crs.Runtime;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;
using System.Security.Claims;

namespace CK.Crs.Tests
{
    public class CommandDecoratorTests
    {
        CancellationTokenSource _cancellationToken;
        public CommandDecoratorTests()
        {
            _cancellationToken = new CancellationTokenSource();
        }

        [Test]
        public async Task Decorators_Should_Be_Invoked_When_A_Command_Is_Handled()
        {
            var decorators = CreateSampleDecorators().ToArray();

            var monitor = TestHelper.Monitor( Console.Out.WriteLine);

            var model = new Handlers.TransferAmountCommand
            {
                SourceAccountId = Guid.NewGuid(),
                DestinationAccountId = Guid.NewGuid(),
                Amount = 1000
            };

            var action = new CommandAction( Guid.NewGuid() )
            {
                Command = model,
                Description = new RoutedCommandDescriptor( "/someroute", new CommandDescription
                {
                    CommandType = model.GetType(),
                    HandlerType = typeof(Handlers.TransferAlwaysSuccessHandler),
                    IsLongRunning = false,
                    Decorators = decorators
                } )
            };

            var command = new CommandExecutionContext( action, monitor, _cancellationToken.Token,
                ( ctx ) => TestHelper.MockEventPublisher(),
                ( ctx ) => TestHelper.MockCommandScheduler() ); 

            var Testory = new FakeTestory<Handlers.TransferAlwaysSuccessHandler>();

            int decoratorInstanciated = 0;
            SampleDecorator sampleDecorator = null;
            Testory.OnDecoratorCreated = ( aDecorator ) =>
            {
                sampleDecorator = aDecorator as SampleDecorator;
                decoratorInstanciated++;
            };

            var strategy = new InProcessExecutionStrategy( new CommandRunner( Testory) );
            var commandContext = new CommandContext( command );
            var response = await strategy.ExecuteAsync( commandContext );

            Assert.AreEqual( 1, decoratorInstanciated );
            Assert.NotNull( sampleDecorator );

            Assert.True( sampleDecorator.OnCommandExecutingCalled );
            Assert.True( sampleDecorator.OnCommandExecutedCalled );
            Assert.False( sampleDecorator.OnExceptionCalled );
        }

        // TODO: replace from Executors decorators lookup
        //[Theory]
        //[InlineData( 2, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[] { typeof( SampleDecorator ) } )]
        //[InlineData( 2, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[] { typeof( SampleDecorator ), typeof( TransactionAttribute ) } )]
        //[InlineData( 3, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[] { typeof( SampleDecorator2 ), typeof( TransactionAttribute ), typeof( SampleDecorator ) } )]
        //[InlineData( 1, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[0] )]
        //[InlineData( 0, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[0] )]
        //[InlineData( 3, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[] { typeof( SampleDecorator2 ), typeof( TransactionAttribute ), typeof( SampleDecorator ) } )]
        //[InlineData( 2, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[] { typeof( SampleDecorator2 ), typeof( TransactionAttribute ) } )]
        //[InlineData( 1, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[] { typeof( TransactionAttribute ) } )]
        //public void Attribute_Lookup_Should_Register_Decorators( int expected, Type commandType, Type handlerType, Type[] globalDecorators )
        //{
        //    CommandReceiverOptions o = new CommandReceiverOptions();
        //    foreach( var g in globalDecorators ) o.AddGlobalDecorator( g );
        //    o.Register( commandType, handlerType, "/c-tests/transferfounds", false );
        //    {
        //        var description = o.Registry.Find( "/c-tests", "transferfounds"  );
        //        Assert.NotNull( description );
        //        Assert.Equal( expected, description.Decorators.Count );
        //    }
        //}

        private IEnumerable<Type> CreateSampleDecorators()
        {
            yield return typeof( SampleDecorator );
        }


        class FakeTestory<THandler> : IFactories where THandler : ICommandHandler, new()
        {
            public ICommandDecorator CreateDecorator( Type type )
            {
                var instance = (ICommandDecorator)Activator.CreateInstance( type );
                OnDecoratorCreated( instance );
                return instance;
            }

            public ICommandHandler CreateHandler( Type handlerType )
            {
                return new THandler();
            }

            public ICommandFilter CreateFilter( Type filterType )
            {
                throw new NotImplementedException();
            }

            public ICommandResponseDispatcher CreateResponseDispatcher()
            {
                return null;
            }

            public IExternalEventPublisher CreateExternalEventPublisher( ICommandExecutionContext context )
            {
                return null;
            }

            public ICommandScheduler CreateCommandScheduler( ICommandExecutionContext context )
            {
                return null;
            }

            public void ReleaseHandler( ICommandHandler handler )
            {
                var d = handler as IDisposable;
                if( d != null ) d.Dispose();
            }

            public Action<ICommandDecorator> OnDecoratorCreated { get; set; }
        }

        class SampleDecorator2 : ICommandDecorator
        {
            public int Order
            {
                get
                {
                    return -10;
                }
            }

            public void OnCommandExecuted( CommandContext ctx )
            {
            }

            public void OnCommandExecuting( CommandContext ctx )
            {
            }

            public void OnException( CommandContext ctx )
            {
            }
        }

        class SampleDecorator : ICommandDecorator
        {
            public bool OnCommandExecutingCalled { get; set; }
            public bool OnCommandExecutedCalled { get; set; }
            public bool OnExceptionCalled { get; set; }

            public int Order { get; set; }

            public void OnCommandExecuting( CommandContext ctx )
            {
                OnCommandExecutingCalled = true;
            }

            public void OnCommandExecuted( CommandContext ctx )
            {
                OnCommandExecutedCalled = true;
            }

            public void OnException( CommandContext ctx )
            {
                OnExceptionCalled = true;
            }
        }
    }
}
