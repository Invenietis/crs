using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CommandReceiverOption
    {
        public CommandReceiverOption( ICommandRegistry r, IAmbientValuesRegistration a )
        {
            Registry = r;
            AmbientValues = a;
        }

        public ICommandRegistry Registry { get; }

        public IAmbientValuesRegistration AmbientValues { get; }
    }
}
