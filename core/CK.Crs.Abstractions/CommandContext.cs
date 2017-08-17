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
        /// <param name="guid"></param>
        /// <param name="activityMonitor"></param>
        /// <param name="model"></param>
        /// <param name="callerId"></param>
        /// <param name="token"></param>
        public CommandContext( Guid guid, IActivityMonitor activityMonitor, CommandModel model, string callerId, CancellationToken token = default( CancellationToken ) )
        {
            CommandId = guid;
            Monitor = activityMonitor ?? throw new ArgumentNullException( nameof( activityMonitor ) );
            Model = model ?? throw new ArgumentNullException( nameof( model ) );
            CallerId = callerId;
            Aborted = token;
        }

        public Guid CommandId { get; }

        public IActivityMonitor Monitor { get; }

        public string CallerId { get; }

        public CancellationToken Aborted { get; }

        public IEndpointModel ReceiverModel { get; }

        public CommandModel Model { get; }
    }
}
