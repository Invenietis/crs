using CK.AspNet.Tester;
using CK.Core;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore.Tests
{
    public static class TestServerJSONExtensions
    {
        public static async Task<Responses.DeferredResponse> PostJSON<T>( this TestServerClient client, Uri uri, T request )
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var command = request;
            var jsonCommand = JsonConvert.SerializeObject( command, settings );
            var response = await client.PostJSONAsync( uri, jsonCommand );
            response.StatusCode.Should().Be( HttpStatusCode.OK );

            var jsonResult = await response.Content.ReadAsStringAsync();
            jsonResult.Should().NotBeEmpty();

            var result = JsonConvert.DeserializeObject<Responses.DeferredResponse>( jsonResult, settings );
            if( result.ResponseType != (char)ResponseType.Asynchronous )
            {
                throw new InvalidOperationException( "Invalid Response. Should be Deferred." );
            }
            return result;
        }

        public static async Task<Response<TResult>> PostJSON<T, TResult>( this TestServerClient client, Uri uri, T request )
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var command = request;
            var jsonCommand = JsonConvert.SerializeObject( command, settings );
            var response = await client.PostJSONAsync( uri, jsonCommand );
            response.StatusCode.Should().Be( HttpStatusCode.OK );

            var jsonResult = await response.Content.ReadAsStringAsync();
            jsonResult.Should().NotBeEmpty();

            var result = JsonConvert.DeserializeObject<Response<TResult>>( jsonResult, settings );
            if( result.ResponseType == (char)ResponseType.InternalError )
            {
                throw new CKException( result.Payload.ToString() );
            }
            return result;
        }
    }
}
