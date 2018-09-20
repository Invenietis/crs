using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( new DefaultHandlerActivator( services ) );
                ITypedCommandHandlerInvoker typedCommandHandlerInvoker = new TypedCommandInvoker( factory );

                await typedCommandHandlerInvoker.Invoke( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandler ) ) );
            }

            using( var services = new SimpleServiceContainer() )
            {
                services.Add( () => new TestAHandlerWithResult() );

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( new DefaultHandlerActivator( services ) );
                ITypedCommandHandlerInvoker typedCommandHandlerInvoker = new TypedCommandInvoker( factory );

                var result = await typedCommandHandlerInvoker.Invoke(
                    new TestACommand(),
                    new TestContext<TestACommand>( typeof( TestAHandlerWithResult ), typeof( TestACommand.Result ) ) );

                Assert.That( result, Is.AssignableFrom<TestACommand.Result>() );
            }
        }

        [Test]
        public async Task factory_tests()
        {
            var services = new ServiceCollection();
            services.AddCrsCore<ScopedAwareActivator>( r => r.Register<TestACommand>().HandledBy<TestAHandlerWithDependency>() );
            services.AddScoped<TestADependency>();
            using( var provider = services.BuildServiceProvider() )
            {
                await provider.GetRequiredService<ICommandReceiver>().ReceiveCommand( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandlerWithDependency ), typeof( TestACommand.Result ) ) );
                await provider.GetRequiredService<ICommandReceiver>().ReceiveCommand( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandlerWithDependency ), typeof( TestACommand.Result ) ) );
                await provider.GetRequiredService<ICommandReceiver>().ReceiveCommand( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandlerWithDependency ), typeof( TestACommand.Result ) ) );

                var result = await Task.WhenAll(
                    Enumerable
                        .Range( 0, 1000 )
                        .Select( i =>
                            provider
                                .GetRequiredService<ICommandReceiver>()
                                .ReceiveCommand( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandlerWithDependency ), typeof( TestACommand.Result ) ) ) ) );

                var numberOfDistinctHash = result.OfType<Response<TestACommand.Result>>().Select( r => r.Payload ).Select( r => r.Hash ).Distinct().Count();
                Assert.That( numberOfDistinctHash, Is.EqualTo( 1000 ) );     
            }
        }


        [Test]
        public async Task wrong_registrations_tests()
        {
            var services = new ServiceCollection();
            services.AddCrsCore( r => r.Register<TestACommand>().HandledBy<TestAHandler>() );
            using( var provider = services.BuildServiceProvider() )
            {
                var commandModel = provider
                    .GetRequiredService<ICommandRegistry>()
                    .GetCommandByName( new CommandName( typeof( TestACommand ) ) );

                await provider
                    .GetRequiredService<ICommandReceiver>()
                    .ReceiveCommand( new TestACommand(), new TestContext<TestACommand>( commandModel ) );
            }
        }

        public class ScopedAwareActivator : ICommandHandlerActivator
        {
            private readonly IServiceScopeFactory _serviceProvider;
            private readonly Dictionary<object, IServiceScope> _scopes;

            public ScopedAwareActivator( IServiceScopeFactory serviceProvider )
            {
                _serviceProvider = serviceProvider;
                _scopes = new Dictionary<object, IServiceScope>();
            }

            public object Create( Type t )
            {
                var scope = _serviceProvider.CreateScope();
                var instance = scope.ServiceProvider.GetService( t ) ?? ActivatorUtilities.CreateInstance( scope.ServiceProvider, t );
                if( instance != null )
                {
                    _scopes[instance] = scope;
                    return instance;
                }
                else
                {
                    scope.Dispose();
                    return null;
                }
            }

            public void Release( object o )
            {
                if( o != null )
                {
                    if( _scopes.TryGetValue( o, out var scope ) )
                    {
                        scope.Dispose();
                    }
                }
            }
        }

        [Test]
        public async Task DefaultInvoker_Tests()
        {
            using( var services = new SimpleServiceContainer() )
            {
                services.Add( () => new TestAHandler() );

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( new DefaultHandlerActivator( services ) );
                ICommandHandlerInvoker commandHandlerInvoker = new TypedCommandInvoker( factory );

                await commandHandlerInvoker.Invoke( new TestACommand(), new TestContext<TestACommand>( typeof( TestAHandler ) ) );
            }

            using( var services = new SimpleServiceContainer() )
            {
                services.Add( () => new TestAHandlerWithResult() );

                ICommandHandlerFactory factory = new DefaultCommandHandlerFactory( new DefaultHandlerActivator( services ) );
                ICommandHandlerInvoker commandHandlerInvoker = new TypedCommandInvoker( factory );

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
                public int Hash { get; set; }
            }
        }

        class TestADependency
        {
        }

        class TestAHandlerWithDependency : ICommandHandler<TestACommand, TestACommand.Result>
        {
            private readonly TestADependency _d;

            public TestAHandlerWithDependency( TestADependency d )
            {
                _d = d;
            }
            public Task<TestACommand.Result> HandleAsync( TestACommand command, ICommandContext context )
            {
                Console.WriteLine( _d.GetHashCode() );
                return Task.FromResult( new TestACommand.Result { Hash = _d.GetHashCode() } );
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
            public TestContext( ICommandModel model )
            {
                Model = model;
                Monitor = new ActivityMonitor();
                Monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            }

            public TestContext( Type handlerType, Type resultType = null )
            {
                Model = new TestCommandModel<T>( handlerType, resultType );
                Monitor = new ActivityMonitor();
                Monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            }

            public string CommandId => Guid.Empty.ToString();

            public IActivityMonitor Monitor { get; }

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
