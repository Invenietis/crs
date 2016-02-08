using System;
using System.Collections.Generic;
using CK.Core;
using System.Linq;

namespace CK.Infrastructure.Commands
{
    public class CommandRegistry : ICommandRegistry
    {
        IEnumerable<CommandDescriptor> _computedMap;
        List<CommandDescriptor> Map { get; } = new List<CommandDescriptor>();

        public IEnumerable<CommandDescriptor> Registration
        {
            get { BuildMap(); return _computedMap; }
        }

        private void BuildMap()
        {
            foreach( var descriptor in Map )
            {
                descriptor.IsLongRunning = descriptor.IsLongRunning && EnableLongRunningCommands;
            }
            _computedMap = Map.ToArray();
        }

        /// <summary>
        /// A long running command will take some times to execute (in seconds)
        /// This is different from a Saga or a CK-Task. Very.
        /// </summary>
        public bool EnableLongRunningCommands { get; set; }

        public void Register( CommandDescriptor descriptor )
        {
            Map.Add( descriptor );
        }
    }
}