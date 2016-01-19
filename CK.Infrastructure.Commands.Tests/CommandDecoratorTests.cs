using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace CK.Infrastructure.Commands.Tests
{
    public class CommandDecoratorTests
    {

        [Fact]
        public async Task Decorators_Should_Be_Invoked_When_A_Command_Is_Handled()
        {
            var decorators = CreateSampleDecorators().ToArray();

            var monitor = CreateMonitor();

            var cmd = new Handlers.TransferAmountCommand
            {
                SourceAccountId = Guid.NewGuid(),
                DestinationAccountId = Guid.NewGuid(),
                Amount = 1000
            };

            var commandDescription = new CommandDescriptor
            {
                CommandType = cmd.GetType(),
                HandlerType = typeof(Handlers.TransferAlwaysSuccessHandler),
                IsLongRunning = false,
                Route = "",
                Decorators = decorators
            };

            var commandContext = new CommandContext<Handlers.TransferAmountCommand>(
                monitor,
                cmd,
                Guid.NewGuid(),
                commandDescription.IsLongRunning,
                "3712");

            var factory = new FakeCommandHandlerFactory<Handlers.TransferAlwaysSuccessHandler>();
            var decoratorFactory = new FakeDecoratorFactory();

            int decoratorInstanciated = 0;
            SampleDecorator sampleDecorator = null;
            decoratorFactory.OnDecoratorCreated = ( aDecorator ) =>
            {
                sampleDecorator = aDecorator as SampleDecorator;
                decoratorInstanciated++;
            };

            var runner = new CommandRunner( factory, decoratorFactory );

            await runner.RunAsync( new CommandExecutionContext( commandDescription, commandContext ) );

            Assert.Equal( 1, decoratorInstanciated );
            Assert.NotNull( sampleDecorator );

            Assert.True( sampleDecorator.OnCommandExecutingCalled );
            Assert.True( sampleDecorator.OnCommandExecutedCalled );
            Assert.False( sampleDecorator.OnExceptionCalled );
        }

        [Theory]
        [InlineData( 2, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[] { typeof( SampleDecorator ) } )]
        [InlineData( 2, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[] { typeof( SampleDecorator ), typeof( TransactionAttribute ) } )]
        [InlineData( 3, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[] { typeof( SampleDecorator2 ), typeof( TransactionAttribute ), typeof( SampleDecorator ) } )]
        [InlineData( 1, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandlerWithDecoration ), new Type[0] )]
        [InlineData( 0, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[0] )]
        [InlineData( 3, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[] { typeof( SampleDecorator2 ), typeof( TransactionAttribute ), typeof( SampleDecorator ) } )]
        [InlineData( 2, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[] { typeof( SampleDecorator2 ), typeof( TransactionAttribute ) } )]
        [InlineData( 1, typeof( Handlers.TransferAmountCommand ), typeof( Handlers.TransferAlwaysSuccessHandler ), new Type[] { typeof( TransactionAttribute ) } )]
        public void Attribute_Lookup_Should_Register_Decorators( int expected, Type commandType, Type handlerType, Type[] globalDecorators )
        {
            CommandReceiverOptions o = new CommandReceiverOptions();
            foreach( var g in globalDecorators ) o.AddGlobalDecorator( g );
            o.Register( commandType, handlerType, "/c-tests/transferfounds", false );
            {
                var description = o.Registry.Find( "/c-tests", "transferfounds"  );
                Assert.NotNull( description );
                Assert.Equal( expected, description.Decorators.Count );
            }
        }

        private IEnumerable<Type> CreateSampleDecorators()
        {
            yield return typeof( SampleDecorator );
        }

        private IActivityMonitor CreateMonitor()
        {
            string path = TestHelper.GeneratorResultPath( GetType().Name );
            var mon = new ActivityMonitor();
            mon.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            mon.Output.RegisterClient( new ActivityMonitorTextWriterClient( s =>
            {
                using( var sw = File.AppendText( path ) )
                {
                    sw.Write( s );
                    sw.Flush();
                }
            } ) );
            return mon;
        }

        class FakeCommandHandlerFactory<T> : ICommandHandlerFactory where T : ICommandHandler, new()
        {
            public ICommandHandler Create( Type handlerType )
            {
                return new T();
            }
        }

        class FakeDecoratorFactory : ICommandDecoratorFactory
        {
            public ICommandDecorator Create( Type type )
            {
                var instance = (ICommandDecorator)Activator.CreateInstance( type );
                OnDecoratorCreated( instance );
                return instance;
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

            public void OnCommandExecuted( CommandExecutionContext ctx )
            {
            }

            public void OnCommandExecuting( CommandExecutionContext ctx )
            {
            }

            public void OnException( CommandExecutionContext ctx )
            {
            }
        }

        class SampleDecorator : ICommandDecorator
        {
            public bool OnCommandExecutingCalled { get; set; }
            public bool OnCommandExecutedCalled { get; set; }
            public bool OnExceptionCalled { get; set; }

            public int Order { get; set; }

            public void OnCommandExecuting( CommandExecutionContext ctx )
            {
                OnCommandExecutingCalled = true;
            }

            public void OnCommandExecuted( CommandExecutionContext ctx )
            {
                OnCommandExecutedCalled = true;
            }

            public void OnException( CommandExecutionContext ctx )
            {
                OnExceptionCalled = true;
            }
        }
    }
}
