using System;
namespace CK.Infrastructure.Commands
{
    public class CommandReceiverOptions
    {
        public ICommandRegistry Registry { get; } = new CommandRegistry();

        public bool EnableLongRunningSupport { get; set; }
    }
}