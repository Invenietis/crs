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

        public static async Task<TResult> PostJSON<T, TResult>( this TestServerClient client, Uri uri, T request )
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var command = request;
            var jsonCommand = JsonConvert.SerializeObject( command, settings );
            var response = await client.PostJSON( uri, jsonCommand );
            response.StatusCode.Should().Be( HttpStatusCode.OK );

            var jsonResult = await response.Content.ReadAsStringAsync();
            jsonResult.Should().NotBeEmpty();

            var result = JsonConvert.DeserializeObject<TResult>( jsonResult, settings );
            return result;
        }
    }
}
