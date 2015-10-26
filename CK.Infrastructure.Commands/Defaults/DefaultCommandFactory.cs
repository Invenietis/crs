using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using System.Threading;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandFactory : ICommandRequestFactory
    {
        readonly ICommandFileWriter _fileWriter;
        public DefaultCommandFactory( ICommandFileWriter fileWriter )
        {
            _fileWriter = fileWriter;
        }

        public async Task<ICommandRequest> CreateCommand( CommandDescriptor routeInfo, HttpRequest request, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            CommandRequest commandRequest = new CommandRequest(routeInfo);
            if( request.HasFormContentType )
            {
                commandRequest.Command = Activator.CreateInstance( commandRequest.CommandDescription.CommandType );

                request.Form = await request.ReadFormAsync( cancellationToken );
                if( request.Form.Files.Count > 0 )
                {
                    foreach( IFormFile file in request.Form.Files )
                    {
                        using( var fs = file.OpenReadStream() )
                        {
                            BlobRef blobRef = await _fileWriter.SaveAsync( fs, file.ContentType, file.ContentDisposition, cancellationToken );
                            commandRequest.AddFile( blobRef );
                        }
                    }

                }
                foreach( var formEntry in request.Form )
                {
                    throw new NotSupportedException( "Form model binding is not supported yet" );
                }
            }
            else if( request.ContentType == "application/json" )
            {
                commandRequest.Command = ReadJsonBody( request.Body, routeInfo.CommandType );
            }
            commandRequest.CallbackId = request.Query["c"];
            return commandRequest;
        }

        protected virtual object ReadJsonBody( Stream requestPayload, Type commandType )
        {
            using( var reader = new StreamReader( requestPayload ) )
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject( json, commandType );
            }
        }
    }
}