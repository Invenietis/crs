﻿using System;
using System.Collections.Generic;
using CK.Core;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public class CommandRegistry : ICommandRegistry
    {
        IServiceCollection _services;
        public CommandRegistry( IServiceCollection services )
        {
            _services = services;
        }

        IEnumerable<CommandDescription> _computedMap;
        List<CommandDescription> Map { get; } = new List<CommandDescription>();

        public IEnumerable<CommandDescription> Registration
        {
            get { BuildMap(); return _computedMap; }
        }

        private void BuildMap()
        {
            if( _computedMap != null )
            {
                //foreach( var descriptor in Map )
                //{
                //    descriptor.IsLongRunning = descriptor.IsLongRunning && EnableLongRunningCommands;
                //}
                _computedMap = Map.ToArray();
            }
        }

        /// <summary>
        /// A long running command will take some times to execute (in seconds)
        /// This is different from a Saga or a CK-Task. Very.
        /// </summary>
        public bool EnableLongRunningCommands { get; set; }

        public void Register( CommandDescription descriptor )
        {
            Map.Add( descriptor );
            _services.AddTransient( descriptor.HandlerType );
        }
    }
}