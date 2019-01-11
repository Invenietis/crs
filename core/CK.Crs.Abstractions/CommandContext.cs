using System;
using System.Collections.Generic;
using System.Threading;
using CK.Core;

namespace CK.Crs
{

    public abstract class CommandContext : ICommandContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="activityMonitor"></param>
        /// <param name="model"></param>
        /// <param name="callerId"></param>
        /// <param name="token"></param>
        public CommandContext( string commandId, IActivityMonitor activityMonitor, ICommandModel model, IEndpointModel endpointModel, CallerId callerId, CancellationToken token = default( CancellationToken ) )
        {
            CommandId = commandId;
            Monitor = activityMonitor ?? throw new ArgumentNullException( nameof( activityMonitor ) );
            Model = model ?? throw new ArgumentNullException( nameof( model ) );
            EndpointModel = endpointModel;
            CallerId = callerId;
            Aborted = token;
        }

        public string CommandId { get; }

        public IActivityMonitor Monitor { get; }

        public CallerId CallerId { get; }

        public CancellationToken Aborted { get; }

        public ICommandModel Model { get; }
        public IEndpointModel EndpointModel { get; }

        Feature _contextFeature;

        private Feature CommandContextFeature
        {
            get => _contextFeature ?? (_contextFeature = new Feature());
        }

        public T GetFeature<T>() where T : class
        {
            return CommandContextFeature.GetFeature<T>();
        }

        public void SetFeature<T>( T feature ) where T : class
        {
            if( feature == null ) CommandContextFeature.RemoveFeature<T>();
            else CommandContextFeature.SetFeature( feature );
        }
    }
}
