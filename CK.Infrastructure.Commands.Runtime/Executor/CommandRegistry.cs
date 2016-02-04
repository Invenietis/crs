using System;
using System.Collections.Generic;
using CK.Core;
using System.Linq;

namespace CK.Infrastructure.Commands
{
    public class CommandRegistry : ICommandRegistry
    {
        List<CommandDescriptor> Map { get; } = new List<CommandDescriptor>();

        public IEnumerable<CommandDescriptor> Registration
        {
            get { return Map; }
        }

        public void Register( CommandDescriptor descriptor )
        {
            Map.Add( descriptor );
        }
    }
}