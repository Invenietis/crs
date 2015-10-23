﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRouteMap
    {
        CommandRouteRegistration FindCommandRoute( string requestPath, ICommandReceiverOptions options );

        ICommandRouteMap Register( CommandRouteRegistration route );
    }
}
