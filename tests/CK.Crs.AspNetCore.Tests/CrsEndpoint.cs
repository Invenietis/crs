using CK.AspNet.Tester;
using CK.Core;
using CK.Crs.Meta;
using CK.Crs.Responses;
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
            //await endpoint.Connection.StartAsync().OrTimeout();
            await endpoint.LoadMeta( "/crs" );
            return endpoint;
        }

        readonly TestServerClient _client;

        CrsEndpoint( PathString path )
        {
            Path = path;

            var builder = new WebHostBuilder()
                .UseUrls( "http://localhost:5001" )
                .UseStartup<Startup>();
            var server = new TestServer( builder );

            _client = new TestServerClient( server, true );

            Services = server.Host.Services;
            //var connectionBuilder = new HubConnectionBuilder()
            //    .WithUrl( _client.BaseAddress + path )
            //    .WithTransport( Microsoft.AspNetCore.Sockets.TransportType.All )
            //    .WithHubProtocol( new JsonHubProtocol( new JsonSerializer() ) );

            //Connection = connectionBuilder.Build();
            //Connection.On<string>( nameof( ICrsHub.ReceiveCallerId ), callerId =>
            //{
            //    CallerId = CallerId.Parse( callerId );
            //} );

            //Connection.Connected += OnConnected;
            //Hub = Services.GetRequiredService<HubEndPoint<CrsHub>>();
            //Hub.OnConnectedAsync( new HubConnectionContext() )
        }

        protected virtual Task OnConnected()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public CallerId CallerId { get; private set; }

        //public HubConnection Connection { get; private set; }

        public MetaCommand.Result Meta { get; private set; }

        public IServiceProvider Services { get; }

        public PathString Path { get; }

        public TestServerClient Client => _client;

        public async Task LoadMeta( PathString crsEndpointPath )
        {
            var result = await Client.PostJSON<MetaCommand, MetaCommand.Result>(
                   new Uri( crsEndpointPath.Add( "/__meta" ), UriKind.Relative ),
                   new MetaCommand { ShowAmbientValues = true, ShowCommands = true } );

            result.CommandId.Should().Be( Guid.Empty.ToString( "N" ) );
            result.ResponseType.Should().Be( (char)ResponseType.Meta );
            result.Payload.AmbientValues.Should().NotBeEmpty();
            result.Payload.Commands.Should().NotBeEmpty();

            Meta = result.Payload;
        }

        public Task<TResult> InvokeCommand<T, TResult>( T command )
        {
            var context = new CKTraitContext( "Crs" );
            var tcs = new TaskCompletionSource<TResult>();
            var commandDescription = Meta.Commands.Where( x => x.Value.CommandType == typeof( T ).FullName ).Select( t => t.Value ).SingleOrDefault();

            var url = Path.Add( "/" + commandDescription.CommandName );
            var uri = new UriBuilder()
            {
                Path = Path.Add( "/" + commandDescription.CommandName ),
                Query = Meta.CallerIdPropertyName + "=" + "3712"
            };

            var ffTag = context.FindOrCreate( CrsTraits.FireForget );
            if( ffTag.Overlaps( context.FindOrCreate( commandDescription.Traits ) ) )
            {

                var task = Client.PostJSON<T>( uri.Uri, command );
                task.ContinueWith( new Action<Task<DeferredResponse>, object>( ( t, state ) =>
                {
                    if( t.Exception != null ) tcs.SetException( t.Exception );
                    else
                    {
                        try
                        {
                            t.Result.CommandId.Should().NotBe( null );
                            t.Result.ResponseType.Should().Be( (char)ResponseType.Asynchronous );
                            t.Result.Payload.Should().BeOfType<string>();
                        }
                        catch( Exception ex )
                        {
                            tcs.SetException( ex );
                        }
                    }

                } ), TaskContinuationOptions.None );

                //Connection.On<string>( nameof( ICrsHub.ReceiveResult ), h =>
                //{
                //    var result = JsonConvert.DeserializeObject<TResult>( h );
                //    tcs.SetResult( result );
                //} );
                //task.Start();
            }
            else
            {

                Client.PostJSON<T, TResult>( uri.Uri, command ).ContinueWith( new Action<Task<Response<TResult>>, object>( ( t, state ) =>
                {
                    if( t.Exception != null )
                    {
                        tcs.SetException( t.Exception );
                    }
                    else if( t.Result is Response<TResult> res )
                    {
                        res.CommandId.Should().NotBe( null );
                        res.ResponseType.Should().Be( (char)ResponseType.Synchronous );
                        res.Payload.Should().BeOfType<TResult>();
                        tcs.SetResult( res.Payload );
                    }
                } ), TaskContinuationOptions.None );

            }
            return tcs.Task;
            //return result.Payload;
        }
        public async Task<string> InvokeFireAndForgetCommand<T>( T command )
        {
            var commandDescription = Meta.Commands.Where( x => x.Key == new CommandName( typeof( T ) ) ).Select( t => t.Value ).SingleOrDefault();
            var uri = new UriBuilder()
            {
                Path = Path.Add( "/" + commandDescription.CommandName )
            };
            var result = await Client.PostJSON<T>( uri.Uri, command );
            result.CommandId.Should().NotBe( null );
            result.ResponseType.Should().Be( (char)ResponseType.Asynchronous );
            result.Payload.Should().BeOfType<string>();

            return result.Payload;
        }
    }
}
