using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Main option root for global CRS configuration
    /// </summary>
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
