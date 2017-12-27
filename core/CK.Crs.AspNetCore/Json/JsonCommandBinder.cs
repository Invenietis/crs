using CK.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore
{
    public class JsonCommandBinder : ICommandBinder
    {
        public string ContentType => "application/json";

        public async virtual Task<object> Bind( ICommandContext commandContext )
        {
            if( commandContext == null )
            {
                throw new ArgumentNullException( nameof( commandContext ) );
            }
            var httpContextFeature = commandContext.GetFeature<IHttpContextCommandFeature>();
            if( httpContextFeature == null ) throw new InvalidOperationException( "The JSON CommandBinder relies on the IHttpContextCommandFeature." );

            var monitor = commandContext.Monitor;
            var httpContext = httpContextFeature.HttpContext;
            if( httpContext.Request.Method != HttpMethod.Post.Method )
            {
                monitor.Warn( $"Receives a non POST request. Binding does not applies." );
                return null;
            }

            using( monitor.OpenInfo( $"Binding command with {typeof( JsonCommandBinder ).Name}" ) )
            {

                var request = httpContext.Request;
                if( !request.ContentType.Contains( ContentType ) )
                {
                    monitor.Warn( $"ContentType application/json expected but received {request.ContentType}" );
                    return null;
                }
                if( request.ContentLength == 0 )
                {
                    monitor.Warn( $"Content is empty or ContentLength not specified." );
                    return null;
                }
                if( request.HasFormContentType )
                {
                    monitor.Warn( $"Form content is not supported by this {typeof( JsonCommandBinder ).Name}" );
                    return null;
                }
                try
                {
                    monitor.Trace( "Reading the body and tries to bind the command" );
                    using( StreamReader sr = new StreamReader( httpContext.Request.Body ) )
                    {
                        var settings = ActivatorUtilities.GetServiceOrCreateInstance<JsonSerializerSettings>( httpContext.RequestServices );

                        return JsonConvert.DeserializeObject(
                            await sr.ReadToEndAsync(),
                            commandContext.Model.CommandType,
                            settings );
                    }
                }
                catch( Exception ex )
                {
                    monitor.Error( ex );
                    return null;
                }

            }
        }
    }
}
