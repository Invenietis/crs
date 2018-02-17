using CK.Core;

namespace CK.Crs.InMemory
{
    class CommandJob
    {
        public object Command { get; set; }

        public CommandJobContext CommandContext { get; set; }
    }
}
