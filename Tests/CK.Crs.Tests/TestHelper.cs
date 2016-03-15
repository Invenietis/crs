using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Moq;
using NUnit.Framework;
using Xunit.Abstractions;

namespace CK.Crs.Tests
{
    class TestAttribute : Xunit.FactAttribute
    {
    }

    public class TestHelper
    {
        public static IServiceProvider CreateServiceProvider( Action<IServiceCollection> setup )
        {
            var services = new ServiceCollection();
            if( setup != null )
            {
                setup( services );
            }

            return services.BuildServiceProvider();
        }
        static IApplicationEnvironment ApplicationEnv
        {
            get { return PlatformServices.Default.Application; }
        }

        public static IActivityMonitor Monitor( Action<string> writer )
        {
            var mon = new ActivityMonitor();
            mon.Output.RegisterClient( new ActivityMonitorTextWriterClient( writer ) );
            return mon;
        }

        internal static string GeneratorResultPath( string testName )
        {
            return Path.Combine( ApplicationEnv.ApplicationBasePath, "TestOutputs", $"{testName}_{DateTime.UtcNow.ToString( "HHmmss" )}.log" );
        }

        public static IExternalEventPublisher MockEventPublisher()
        {
            var e = new Mock<IExternalEventPublisher>();
            e.Setup( x => x.Push( It.IsAny<object>() ) );
            e.Setup( x => x.ForcePush( It.IsAny<object>() ) );
            return e.Object;
        }

        internal static ICommandScheduler MockCommandScheduler()
        {
            var e = new Mock<ICommandScheduler>();
            e.Setup( x => x.Schedule( It.IsAny<object>(), It.IsAny<CommandSchedulingOption>() ) ).Returns( () => Guid.NewGuid() );
            e.Setup( x => x.CancelScheduling( It.IsAny<Guid>() ) ).Returns( false );
            return e.Object;
        }

        internal static IAmbientValues CreateAmbientValues( IServiceProvider sp = null )
        {
            sp = sp ?? TestHelper.CreateServiceProvider( Util.ActionVoid );

            IAmbientValues ambientValues = new AmbientValues( new DefaultAmbientValueFactory( sp) );

            return ambientValues;
        }

        internal CommandExecutionContext CreateContext<T>( T command, ITestOutputHelper output, CancellationToken token = default( CancellationToken ) ) where T : class
        {
            var monitor = Monitor( output.WriteLine );

            var action = new CommandAction( Guid.NewGuid() )
            {
                Command = command,
                Description = new RoutedCommandDescriptor( "/someroute", new CommandDescription
                {
                    CommandType = command.GetType(),
                    HandlerType = typeof( ICommandHandler<T> ),
                    IsLongRunning = false
                } )
            };

            var ctx = new CommandExecutionContext( action, monitor,token,
                ( cctx ) => TestHelper.MockEventPublisher(),
                ( cctx ) => TestHelper.MockCommandScheduler() );

            return ctx;
        }
    }
}
