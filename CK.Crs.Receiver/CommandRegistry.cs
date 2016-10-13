using System;
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

        List<CommandDescription> Map { get; } = new List<CommandDescription>();

        public IEnumerable<CommandDescription> Registration
        {
            get { return Map; }
        }

        public void Register( CommandDescription descriptor )
        {
            Map.Add( descriptor );
            _services.AddTransient( descriptor.HandlerType );
        }
    }
}