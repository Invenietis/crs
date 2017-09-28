using CK.Core;

namespace CK.Crs.InMemory
{
    class CommandJob
    {
        public object Command { get; set; }

        public ICommandContext CommandContext { get; set; }

        public ActivityMonitor.DependentToken Token { get; set; }
    }
}
