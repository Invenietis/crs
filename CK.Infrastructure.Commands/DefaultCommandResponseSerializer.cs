using System;
using System.IO;
using System.Threading.Tasks;
using CK.Infrastructure.Commands;
using Newtonsoft.Json;

namespace CK.Infrastructure.Commands
{
    internal class DefaultCommandResponseSerializer : ICommandResponseSerializer
    {
        Newtonsoft.Json.JsonSerializer _serializer;
        public DefaultCommandResponseSerializer()
        {
            _serializer = Newtonsoft.Json.JsonSerializer.Create();
        }

        public void Serialize( ICommandResponse response, Stream outputStream )
        {
            StreamWriter sw = new StreamWriter( outputStream );
            JsonTextWriter jw = new JsonTextWriter( sw );

            var directResponse = new DirectResponse
            {
                CommandId = response.CommandId.ToString(),
                Payload = response.Payload
            };
            _serializer.Serialize( jw, directResponse );
            jw.Flush();
        }

        class DirectResponse
        {
            public string CommandId { get; set; }

            public object Payload { get; set; }
        }
    }
}