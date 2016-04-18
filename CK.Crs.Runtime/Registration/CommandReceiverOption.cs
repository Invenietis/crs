using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CommandReceiverOption
    {
        public CommandReceiverOption( ICommandRegistry r )
        {
            Registry = r;
        }

        public ICommandRegistry Registry { get; }
    }
}
