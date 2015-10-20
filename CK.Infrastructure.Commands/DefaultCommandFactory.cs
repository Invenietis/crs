using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandFactory : CommandFactoryBase
    {
        protected override ICommand Deserialize( string json, Type t )
        {
            ICommand command = (ICommand)JsonConvert.DeserializeObject( json, t );
            Debug.Assert( command != null );
            return command;
        }
    }
}