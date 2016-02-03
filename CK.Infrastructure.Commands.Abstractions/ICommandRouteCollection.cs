﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRouteCollection
    {
        RoutedCommandDescriptor FindCommandDescriptor( string path );

        void AddCommandRoute( CommandDescriptor descriptor );
    }
}