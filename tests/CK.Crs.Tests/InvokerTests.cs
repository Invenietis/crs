using CK.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Tests
{
    public class InvokerTests
    {
        [Test]
        public async Task TypedInvoker_Tests()
        {
            using( var services = new SimpleServiceContainer() )
            {
                services.Add( () => new TestAHandler() );

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( services );
                ITypedCommandHandlerInvoker typedCommandHandlerInvoker = new TypedCommandInvoker( factory );

                await typedCommandHandlerInvoker.InvokeGeneric( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandler ) ) );
            }

            using( var services = new SimpleServiceContainer() )
            {
                services.Add( () => new TestAHandlerWithResult() );

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( services );
                ITypedCommandHandlerInvoker typedCommandHandlerInvoker = new TypedCommandInvoker( factory );

                var result = await typedCommandHandlerInvoker.InvokeGeneric(
                    new TestACommand(),
                    new TestContext<TestACommand>( typeof( TestAHandlerWithResult ), typeof( TestACommand.Result ) ) );

                Assert.That( result, Is.AssignableFrom<TestACommand.Result>() );
            }
        }

        [Test]
        public async Task DefaultInvoker_Tests()
        {
            using( var services = new SimpleServiceContainer() )
            {
                services.Add( () => new TestAHandler() );

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( services );
                ICommandHandlerInvoker commandHandlerInvoker = new DefaultCommandInvoker( factory );

                await commandHandlerInvoker.Invoke( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandler ) ) );
            }

            using( var services = new SimpleServiceContainer() )
            {
                services.Add( () => new TestAHandlerWithResult() );

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( services );
                ICommandHandlerInvoker commandHandlerInvoker = new DefaultCommandInvoker( factory );

                var result = await commandHandlerInvoker.Invoke(
                    new TestACommand(),
                    new TestContext<TestACommand>( typeof( TestAHandlerWithResult ), typeof( TestACommand.Result ) ) );

                Assert.That( result, Is.AssignableFrom<TestACommand.Result>() );
            }
        }


        class TestACommand
        {
            public class Result
            {
            }
        }

        class TestAHandler : ICommandHandler<TestACommand>
        {
            public Task HandleAsync( TestACommand command, ICommandContext context )
            {
                return Task.CompletedTask;
            }
        }
        class TestAHandlerWithResult : ICommandHandler<TestACommand, TestACommand.Result>
        {
            public Task<TestACommand.Result> HandleAsync( TestACommand command, ICommandContext context )
            {
                return Task.FromResult<TestACommand.Result>( new TestACommand.Result() );
            }
        }
        class TestContext<T> : ICommandContext
        {
            public TestContext( Type handlerType, Type resultType = null )
            {
                Model = new TestCommandModel<T>( handlerType, resultType );
            }

            public string CommandId => Guid.Empty.ToString();

            public IActivityMonitor Monitor => new ActivityMonitor();

            public CancellationToken Aborted => default( CancellationToken );

            public CallerId CallerId => CallerId.None;

            public ICommandModel Model { get; }

            public T1 GetFeature<T1>() where T1 : class
            {
                return null;
            }

            public void SetFeature<T1>( T1 feature ) where T1 : class
            {
            }
        }

        class TestCommandModel<T> : ICommandModel
        {
            public TestCommandModel( Type handlerType, Type resultType )
            {
                HandlerType = handlerType;
                ResultType = resultType;
            }
            public CommandName Name => new CommandName( typeof( T ).Name );

            public CKTrait Tags { get; set; }

            public Type CommandType => typeof( T );

            public string Description => null;

            public Type HandlerType { get; }

            public Type ResultType { get; }

            public IEnumerable<Type> Filters { get; }

            public Type Binder { get; }
        }
    }
}
