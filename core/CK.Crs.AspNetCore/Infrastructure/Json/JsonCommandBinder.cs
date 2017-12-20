using CK.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class JsonCommandBinder : ICommandBinder
    {
        public string ContentType => "application/json";

        public HttpContext Context { get; }

        public JsonCommandBinder( HttpContext context )
        {
            Context = context;
        }

        public async Task<object> Bind( ICommandContext commandContext )
        {
            if( commandContext == null )
            {
                throw new ArgumentNullException( nameof( commandContext ) );
            }

            var monitor = commandContext.Monitor;
            using( monitor.OpenInfo( $"Binding command with {typeof( JsonCommandBinder ).Name}" ) )
            {

                var request = Context.Request;
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
                    using( StreamReader sr = new StreamReader( Context.Request.Body ) )
                    {
                        var settings = ActivatorUtilities.GetServiceOrCreateInstance<JsonSerializerSettings>( Context.RequestServices );

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
