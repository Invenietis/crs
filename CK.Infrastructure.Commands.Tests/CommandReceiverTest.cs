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
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CK.Core;
using CK.Infrastructure.Commands.Tests.Handlers;
using Microsoft.Extensions.OptionsModel;

namespace CK.Infrastructure.Commands.Tests
{
    public class SetupFixture : IDisposable
    {
        public void Dispose()
        {
        }
    }

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
                services.AddCommandReceiver();
                services.AddSingleton<ICommandResponseDispatcher, EventChannel>();
            } );
        }

        public IServiceProvider ApplicationServices { get; private set; }

        public RequestDelegate SuccessDelegate { get; } = new RequestDelegate( ( HttpContext httpContext ) =>
        {
            return Task.FromResult<object>( null );
        } );

        public RequestDelegate ShouldNotInvokeDelegate { get; } = new RequestDelegate( ( HttpContext httpContext ) =>
        {
            throw new CKException( "Next delegate invoked." );
        } );

        [Fact]
        public async Task CommandReceiver_Should_Be_Scoped_To_A_Route_Prefix()
        {
            var cmd =new Handlers.TransferAmountCommand
            {
                SourceAccountId = Guid.NewGuid(),
                DestinationAccountId = Guid.NewGuid(),
                Amount = 1000
            };
            OptionStub option = new OptionStub();
            {
                Commands.CommandReceiver r = new Commands.CommandReceiver( ShouldNotInvokeDelegate, option, null  );
                using( var httpContext = new FakeHttpContext( ApplicationServices, "/api", SerializeRequestBody( cmd ) ) )
                {
                    var exc = await Assert.ThrowsAsync<CKException>( () => r.Invoke( httpContext ) );
                    Assert.Equal( "Next delegate invoked.", exc.Message );
                }

                option.Value.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>( typeof( TransferAmountCommand ).Name, false );
                using( var httpContext = new FakeHttpContext( ApplicationServices, "/api", SerializeRequestBody( cmd ) ) )
                {
                    var exc = await Assert.ThrowsAsync<CKException>( () => r.Invoke( httpContext ) );
                    Assert.Equal( "Next delegate invoked.", exc.Message );
                }
            }
            {
                Commands.CommandReceiver r = new Commands.CommandReceiver( SuccessDelegate, option, "/c" );

                using( var httpContext = new FakeHttpContext( ApplicationServices, "/c/TransferAmountCommand", SerializeRequestBody( cmd ) ) )
                {
                    await r.Invoke( httpContext );
                    Assert.NotNull( httpContext.Response.Body );
                    Assert.True( httpContext.Response.Body.Length > 0 );
                }
            }
        }

        private Stream SerializeRequestBody<T>( T command )
        {
            Stream s = new MemoryStream();
            StreamWriter sw = new StreamWriter(s);
            Newtonsoft.Json.JsonSerializer.Create().Serialize( sw, command );
            s.Seek( 0, SeekOrigin.Begin );
            return s;
        }
    }

}
