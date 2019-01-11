using Rebus.Pipeline;
using System.Threading;

namespace CK.Crs.Rebus
{
    class RebusCommandContext : CommandContext
    {
        public RebusCommandContext( IMessageContext msgContext, ICommandModel model, CancellationTokenSource token )
            :base( msgContext.GetCommandId(), msgContext.GetActivityMonitor(), model, null, msgContext.GetCallerId(), token.Token )
        {
        }
    }
}
