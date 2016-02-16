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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandBinder : ICommandBinder
    {
        readonly IServiceProvider _serviceProvider;
        readonly ICommandFileWriter _fileWriter;

        public DefaultCommandBinder( ICommandFileWriter fileWriter, IServiceProvider serviceProvider )
        {
            _fileWriter = fileWriter;
            _serviceProvider = serviceProvider;
        }


        public async Task<ICommandRequest> BindCommand( RoutedCommandDescriptor descriptor, HttpRequest request, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            CommandRequest commandRequest = new CommandRequest(descriptor);
            commandRequest.Command = CreateCommand( commandRequest.CommandDescription.Descriptor.CommandType );

            if( request.HasFormContentType )
            {

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
                PopulateFromJsonRequest( commandRequest.Command, request.Body );
            }
            else throw new NotSupportedException( $"The content type {request.ContentType} is not supported yet." );
            commandRequest.CallbackId = request.Query["c"];
            return commandRequest;
        }

        protected virtual object CreateCommand( Type commandType )
        {
            return ActivatorUtilities.CreateInstance( _serviceProvider, commandType );
        }

        protected virtual void PopulateFromJsonRequest( object command, Stream requestPayload )
        {
            using( var reader = new StreamReader( requestPayload ) )
            {
                JsonConvert.PopulateObject( reader.ReadToEnd(), command );
            }
        }
    }
}