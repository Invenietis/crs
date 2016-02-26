using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Moq;

namespace CK.Crs.Tests
{
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
    }
}
