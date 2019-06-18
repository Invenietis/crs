using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CK.Crs.Owin
{
    public class JsonCommandBinder : ICommandBinder
    {
        private readonly IServiceProvider _applicationServices;
        private JsonSerializerSettings _settings;

        public JsonCommandBinder( JsonSerializerSettings settings )
        {
            _settings = settings;
        }

        public string ContentType => "application/json";

        public async virtual Task<object> Bind( ICommandContext commandContext )
        {
            if( commandContext == null )
            {
                throw new ArgumentNullException( nameof( commandContext ) );
            }
            var owinContextFeature = commandContext.GetFeature<IOwinContextCommandFeature>();
            if( owinContextFeature == null ) throw new InvalidOperationException( "The JSON CommandBinder relies on the IHttpContextCommandFeature." );

            var monitor = commandContext.Monitor;
            var owinContext = owinContextFeature.OwinContext;
            if( owinContext.Request.Method != HttpMethod.Post.Method )
            {
                monitor.Warn( $"Receives a non POST request. Binding does not applies." );
                return null;
            }

            using( monitor.OpenInfo( $"Binding command with {typeof( JsonCommandBinder ).Name}" ) )
            {

                var request = owinContext.Request;
                if( !request.ContentType.Contains( ContentType ) )
                {
                    monitor.Warn( $"ContentType application/json expected but received {request.ContentType}" );
                    return null;
                }
                if(
                    string.IsNullOrEmpty( request.Headers.Get( "Content-Length" ) )
                    || int.Parse( request.Headers.Get( "Content-Length" ) ) == 0
                    )
                {
                    monitor.Warn( $"Content is empty or ContentLength not specified." );
                    return null;
                }
                if( HasFormContentType( request ) )
                {
                    monitor.Warn( $"Form content is not supported by this {typeof( JsonCommandBinder ).Name}" );
                    return null;
                }
                try
                {
                    monitor.Trace( "Reading the body and tries to bind the command" );
                    using( StreamReader sr = new StreamReader( owinContext.Request.Body ) )
                    {
                        return JsonConvert.DeserializeObject( await sr.ReadToEndAsync().ConfigureAwait( false ), commandContext.Model.CommandType, _settings );
                    }
                }
                catch( Exception ex )
                {
                    monitor.Error( ex );
                    return null;
                }

            }
        }
        public static bool HasFormContentType( IOwinRequest req )
        {
            return !string.IsNullOrEmpty( req.ContentType ) && (
                req.ContentType.Equals( "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase )
                || req.ContentType.Equals( "multipart/form-data", StringComparison.OrdinalIgnoreCase )
                );
        }
    }

}
