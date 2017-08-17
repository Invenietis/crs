using CK.Core;
using Rebus.Pipeline;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class RebusExtensions
    {
        public static bool HasAsyncTag( this CommandModel commandModel )
        {
            CKTrait asyncTrait = commandModel.Tags.Context.FindOrCreate( "Async" );
            return commandModel.Tags.Overlaps( asyncTrait );
        }

        public static ICommandRegistration IsAsync( this ICommandRegistration commandRegistration )
        {
            CKTrait asyncTrait = commandRegistration.Model.Tags.Context.FindOrCreate( "Async" );
            commandRegistration.Model.Tags = commandRegistration.Model.Tags.Apply( asyncTrait, SetOperation.Union );
            return commandRegistration;
        }

        /// <summary>
        /// Gets an <see cref="IActivityMonitor"/> from <see cref="IMessageContext"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IActivityMonitor GetActivityMonitor( this IMessageContext context )
        {
            return context.TransactionContext.GetOrAdd( nameof( ICommandContext.Monitor ), () =>
            {
                var monitor = new ActivityMonitor();
                var monitorToken = context.Headers[nameof( ICommandContext.Monitor )];
                if( monitorToken != null )
                {
                    var monitorDisposable = monitor.StartDependentActivity( ActivityMonitor.DependentToken.Parse( monitorToken ) );
                    context.TransactionContext.OnDisposed( () => monitorDisposable.Dispose() );
                }
                return monitor;
            } );
        }


        public static Guid GetCommandId( this IMessageContext context )
        {
            return context.TransactionContext.GetOrAdd( nameof( ICommandContext.CommandId ), () =>
            {
                var commandId = context.Headers[nameof( ICommandContext.CommandId )];
                if( commandId != null )
                {
                    return Guid.Parse( commandId );
                }
                return Guid.Empty;
            } );

        }

        public static string GetCallerId( this IMessageContext context )
        {
            return context.TransactionContext.GetOrAdd( nameof( ICommandContext.CallerId ), () =>
            {
                return context.Headers[nameof( ICommandContext.CallerId )];
            } );
        }
    }
}
