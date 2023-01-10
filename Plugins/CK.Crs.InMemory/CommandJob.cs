using CK.Core;

namespace CK.Crs.InMemory
{
    class CommandJob : ICommandJob
    {
        public object Command { get; set; }

        public CommandJobContext CommandContext { get; set; }

        ICommandContext ICommandJob.CommandContext => CommandContext;
    }
}
