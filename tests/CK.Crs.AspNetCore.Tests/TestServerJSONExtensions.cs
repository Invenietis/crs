using CK.AspNet.Tester;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore.Tests
{
    public static class TestServerJSONExtensions
    {
        public static async Task<MetaCommand.Result> GetMeta( this TestServerClient client, PathString crsEndpointPath )
        {
            var result = await client.PostJSON<MetaCommand, MetaCommand.Result>(
                   crsEndpointPath.Add( "/__meta" ),
                   new MetaCommand { ShowAmbientValues = true, ShowCommands = true } );

            result.RequestId.Should().Be( Guid.Empty );
            result.ResponseType.Should().Be( (char)ResponseType.Meta );
            result.Payload.AmbientValues.Should().NotBeEmpty();
            result.Payload.Commands.Should().NotBeEmpty();

            return result.Payload;
        }

        public static async Task<TResult> InvokeCommand<T, TResult>(
            this TestServerClient client,
            PathString crsEndpointPath,
            T command,
            MetaCommand.Result.MetaCommandDescription commandDescription )
        {
            var url = crsEndpointPath.Add( "/" + commandDescription.CommandName );
            var result = await client.PostJSON<T, TResult>( url, command );
            result.RequestId.Should().NotBe( Guid.Empty );
            result.ResponseType.Should().Be( (char)ResponseType.Synchronous );
            result.Payload.Should().BeOfType<TResult>();
            return result.Payload;
        }

        public static async Task<Response<TResult>> PostJSON<T, TResult>( this TestServerClient client, string url, T request )
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var command = request;
            var jsonCommand = JsonConvert.SerializeObject( command, settings );
            var response = await client.PostJSON( url, jsonCommand );
            response.StatusCode.Should().Be( HttpStatusCode.OK );

            var jsonResult = await response.Content.ReadAsStringAsync();
            jsonResult.Should().NotBeEmpty();

            var result = JsonConvert.DeserializeObject<Response<TResult>>( jsonResult, settings );
            return result;
        }
    }
}
