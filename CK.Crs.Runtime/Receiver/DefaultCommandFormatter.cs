using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace CK.Crs.Runtime
{
    public class DefaultCommandFormatter : ICommandFormatter
    {
        readonly JsonSerializer _serializer;

        public DefaultCommandFormatter()
        {
            _serializer = Newtonsoft.Json.JsonSerializer.Create( new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            } );
        }

        public CommandRequest Deserialize( RoutedCommandDescriptor description, Stream inputStream, IDictionary<string, object> metadata )
        {
            CommandRequest commandRequest = new CommandRequest
            {
                Command = CreateCommand( description.Descriptor.CommandType ),
                CommandDescription = description
            };
            object cId;
            metadata.TryGetValue( "c", out cId );

            if( cId != null )
                commandRequest.CallbackId = cId.ToString();

            Bind( commandRequest.Command, inputStream );
            return commandRequest;
        }

        public void Serialize( CommandResponse response, Stream outputStream )
        {
            using( StreamWriter sw = new StreamWriter( outputStream, leaveOpen: true, bufferSize: 1024, encoding: Encoding.UTF8 ) )
            {
                using( JsonTextWriter jw = new JsonTextWriter( sw ) )
                {
                    _serializer.Serialize( jw, response );
                    jw.Flush();
                }
            }
        }

        protected virtual object CreateCommand( Type commandType )
        {
            return Activator.CreateInstance( commandType );
        }

        protected virtual void Bind( object command, Stream requestPayload )
        {
            using( var reader = new StreamReader( requestPayload ) )
            {
                _serializer.Populate( reader, command );
            }
        }
    }
}