using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{

    public class CommandContext : ICommandContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="activityMonitor"></param>
        /// <param name="model"></param>
        /// <param name="callerId"></param>
        /// <param name="token"></param>
        public CommandContext( string commandId, IActivityMonitor activityMonitor, CommandModel model, CallerId callerId, CancellationToken token = default( CancellationToken ) )
        {
            CommandId = commandId;
            Monitor = activityMonitor ?? throw new ArgumentNullException( nameof( activityMonitor ) );
            Model = model ?? throw new ArgumentNullException( nameof( model ) );
            CallerId = callerId;
            Aborted = token;
        }

        public string CommandId { get; }

        public IActivityMonitor Monitor { get; }

        public CallerId CallerId { get; }

        public CancellationToken Aborted { get; }

        public IEndpointModel ReceiverModel { get; }

        public CommandModel Model { get; }
    }
}
