using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CK.Infrastructure.Commands;
using CK.Infrastructure.Commands.Tests.Fake;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features;
using Microsoft.Dnx.Runtime.Infrastructure;
using Xunit;
using Microsoft.Framework.DependencyInjection;

namespace CK.Infrastructure.Commands.Tests
{
    public class SetupFixture : IDisposable
    {
        public void Dispose()
        {
        }
    }

    [CollectionDefinition( "CK.Infrastructure.Commands.Tests collection" )]
    public class CommandReceiverTestCollection : ICollectionFixture<SetupFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.

        // More information: http://xunit.github.io/docs/shared-context.html
    }

    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    [Collection( "CK.Infrastructure.Commands.Tests collection" )]
    public class CommandReceiverTest
    {
        public CommandReceiverTest( SetupFixture fixture )
        {
            ApplicationServices = TestHelper.CreateServiceProvider( services =>
            {
                services.AddSingleton<ICommandBus, FakeCommandBus>();
                services.AddSingleton<ICommandFactory, FakeCommandFactory>();
                services.AddSingleton<ICommandResultSerializer, FakeCommandResultSerializer>();
                services.AddSingleton<ICommandRouteMap, DefaultCommandRouteMap>();
                services.AddSingleton<ICommandTypeSelector, ConventionCommandTypeSelector>();
            } );
        }

        public IServiceProvider ApplicationServices { get; private set; }

        public RequestDelegate SuccessDelegate { get; } = new RequestDelegate( ( HttpContext httpContext ) =>
        {
            return Task.FromResult<object>( null );
        } );

        public RequestDelegate ShouldNotInvokeDelegate { get; } = new RequestDelegate( ( HttpContext httpContext ) =>
        {
            throw new ApplicationException( "Next delegate invoked." );
        } );


        [Fact]
        public async Task CommandReceiver_Should_Be_Scoped_To_A_Route_Prefix()
        {
            ICommandReceiverOptions options = new DefaultReceiverOptions("/c");
            {
                CommandReceiver r = new CommandReceiver( ShouldNotInvokeDelegate, options);
                ApplicationException exc = await Assert.ThrowsAsync<ApplicationException>( () => r.Invoke( new FakeHttpContext( ApplicationServices, "/api", SerializeRequestBody() ) ) );
                Assert.Equal( "Next delegate invoked.", exc.Message );

                ApplicationServices.GetService<ICommandRouteMap>().Register( new CommandRouteRegistration
                {
                    CommandType = typeof( TestCommand1 ),
                    IsLongRunning = false,
                    Route = "/c/TestCommand1"
                } );

                exc = await Assert.ThrowsAsync<ApplicationException>( () => r.Invoke( new FakeHttpContext( ApplicationServices, "/api", SerializeRequestBody() ) ) );
                Assert.Equal( "Next delegate invoked.", exc.Message );
            }
            {
                CommandReceiver r = new CommandReceiver( SuccessDelegate, options);

                await r.Invoke( new FakeHttpContext( ApplicationServices, "/c/TestCommand1", SerializeRequestBody() ) );
            }

        }

        private Stream SerializeRequestBody()
        {
            return Stream.Null;
        }
    }

}
