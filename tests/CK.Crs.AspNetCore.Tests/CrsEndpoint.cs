using CK.AspNet.Tester;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore.Tests
{
    public class CrsEndpoint : IDisposable
    {
        public static async Task<CrsEndpoint> Create( PathString path )
        {
            CrsEndpoint endpoint = new CrsEndpoint( path );
            await endpoint.LoadMeta( "/crs" );
            return endpoint;
        }

        readonly TestServerClient _client;

        public MetaCommand.Result Meta { get; private set; }

        CrsEndpoint( PathString path, string clientId = null )
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            _client = new TestServerClient( new TestServer( builder ), true );
            ClientId = clientId ?? Guid.NewGuid().ToString( "N" );
            Path = path;
        }

        public PathString Path { get; }

        public TestServerClient Client => _client;

        public async Task LoadMeta( PathString crsEndpointPath )
        {
            var result = await Client.PostJSON<MetaCommand, Response<MetaCommand.Result>>(
                   new Uri( crsEndpointPath.Add( "/__meta" ), UriKind.Relative ),
                   new MetaCommand { ShowAmbientValues = true, ShowCommands = true } );

            result.RequestId.Should().Be( Guid.Empty );
            result.ResponseType.Should().Be( (char)ResponseType.Meta );
            result.Payload.AmbientValues.Should().NotBeEmpty();
            result.Payload.Commands.Should().NotBeEmpty();

            Meta = result.Payload;
        }

        public string ClientId { get; }

        public Response LastResponse { get; private set; }

        public async Task<TResult> InvokeCommand<T, TResult>( T command )
        {
            var commandDescription = Meta.Commands.Where( x => x.Key == new CommandName( typeof( T ) ) ).Select( t => t.Value ).SingleOrDefault();
            var url = Path.Add( "/" + commandDescription.CommandName );
            var uri = new UriBuilder()
            {
                Path = Path.Add( "/" + commandDescription.CommandName ),
                Query = "CallerId=" + ClientId
            };
            var result = await Client.PostJSON<T, Response<TResult>>( uri.Uri, command );
            result.RequestId.Should().NotBe( Guid.Empty );
            result.ResponseType.Should().Be( (char)ResponseType.Synchronous );
            result.Payload.Should().BeOfType<TResult>();

            LastResponse = result;

            return result.Payload;
        }
        public async Task<string> InvokeFireAndForgetCommand<T, TResult>( T command )
        {
            var commandDescription = Meta.Commands.Where( x => x.Key == new CommandName( typeof( T ) ) ).Select( t => t.Value ).SingleOrDefault();
            var uri = new UriBuilder()
            {
                Path = Path.Add( "/" + commandDescription.CommandName ),
                Query = "CallerId=" + ClientId
            };
            var result = await Client.PostJSON<T, DeferredResponse>( uri.Uri, command );
            result.RequestId.Should().NotBe( Guid.Empty );
            result.ResponseType.Should().Be( (char)ResponseType.Asynchronous );
            result.Payload.Should().BeOfType<string>();

            LastResponse = result;

            return result.Payload;
        }
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
