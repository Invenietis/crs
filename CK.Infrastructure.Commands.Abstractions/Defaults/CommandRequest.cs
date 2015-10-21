using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{

    public class CommandRequest : ICommandRequest
    {
        public CommandRequest( object command )
        {
            Command = command;
        }

        public object Command { get; set; }

        public string CallbackId { get; set; }

        public Type CommandServerType { get; set; }
    }

}
