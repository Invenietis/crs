using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CK.Crs.Runtime
{
    public abstract class CommandResponse
    {
        internal CommandResponse( Guid commandId )
        {
            CommandId = commandId;
        }

        public Guid CommandId { get; private set; }

        public CommandResponseType ResponseType { get; protected set; }

        public object Payload { get; protected set; }

        public virtual void Write( Stream responseBody )
        {
            using( StreamWriter sw = new StreamWriter( responseBody, encoding: Encoding.UTF8 ) )
            {
                string json = JsonConvert.SerializeObject( this );
                sw.Write( json );
            }
        }
    }
}
