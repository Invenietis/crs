using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Windsor;
using CK.Core;
using CK.Crs.Runtime.Pipeline;
using Moq;
using NUnit.Framework;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Features.ResolveAnything;

namespace CK.Crs.Tests
{
    [TestFixture]
    public class PipelineComponentCreation
    {
        [Test]
        public async Task pipeline_default_Testory_tests()
        {
            ActivityMonitor.AutoConfiguration = ( monitor ) =>
            {
                monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            };

            CommandReceiverConfiguration config = new CommandReceiverConfiguration("/test", Mock.Of<ICommandRegistry>() );
            config.Pipeline.Clear().UseJsonCommandBuilder();

            var request = new CommandRequest( "/test/lol", Stream.Null, ClaimsPrincipal.Current, "c");
            var serviceCollection = new ServiceCollection();
            
            {
                ContainerBuilder builder = new Autofac.ContainerBuilder();
                builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>();
                builder.RegisterType<AutofacServiceScopeFactory>().As<IServiceScopeFactory>();
                builder.RegisterSource( new AnyConcreteTypeNotAlreadyRegisteredSource() );
                IContainer container = builder.Build();

                var commandReceiver = new CommandReceiver( container.Resolve<IServiceProvider>(),container.Resolve<IServiceScopeFactory>(),  config );
                var response = await commandReceiver.ProcessCommandAsync( request );
                Assert.That( response, Is.Null );
            }
        }

        class AutofacServiceProvider : IServiceProvider
        {
            readonly IActivityMonitor _monitor;
            readonly IComponentContext _componentContext;

            public AutofacServiceProvider( IComponentContext componentContext )
            {
                _monitor = new ActivityMonitor();
                _componentContext = componentContext;
            }

            public object GetService( Type serviceType )
            {
                _monitor.Trace().Send( "Resolving {0}.", serviceType.FullName );
                var result = _componentContext.Resolve( serviceType );
                if( result == null ) _monitor.Warn().Send( "Unable to resolve the type." );

                return result;
            }
        }

        class AutofacServiceScopeFactory : IServiceScopeFactory
        {
            readonly IActivityMonitor _monitor;
            readonly ILifetimeScope _scope;

            IDisposable _group;

            public AutofacServiceScopeFactory( ILifetimeScope scope )
            {
                _monitor = new ActivityMonitor();
                _scope = scope;
            }

            public IServiceScope CreateScope()
            {
                _group = _monitor.OpenTrace().Send( "New scope created..." );
                var lifetimeScope = _scope.BeginLifetimeScope();
                lifetimeScope.Disposer.AddInstanceForDisposal( _group );
                return new AutofacServiceScope( _monitor, lifetimeScope );
            }

            class AutofacServiceScope : IServiceScope
            {
                ILifetimeScope _scope;
                public AutofacServiceScope( IActivityMonitor monitor, ILifetimeScope scope )
                {
                    _scope = scope;
                    ServiceProvider = _scope.Resolve<IServiceProvider>();
                }

                public IServiceProvider ServiceProvider { get; }

                public void Dispose()
                {
                    _scope.Dispose();
                }
            }
        }
    }
}
