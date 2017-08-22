using CK.Core;
using Rebus.Pipeline;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Text;
using R = Rebus;

namespace CK.Crs
{
    public static class RebusExtensions
    {
        public static readonly string CrsPrefix = "CRS_";
        public static readonly string RebusTag = "Rebus";

        public static bool HasRebusTag( this CommandModel commandModel )
        {
            return commandModel.HasTags( RebusTag, CrsTraits.FireForget );
        }

        public static ICommandRegistration IsRebus( this ICommandRegistration commandRegistration )
        {
            return commandRegistration.SetTag( RebusTag, CrsTraits.FireForget );
        }

        public static Dictionary<string, string> CreateHeaders( this ICommandContext context )
        {
            return new Dictionary<string, string>
            {
                // CRS Headers
                { CrsPrefix + "CommandName", context.Model.Name },
                { CrsPrefix + nameof( ICommandContext.CallerId ), context.CallerId },
                { CrsPrefix + nameof( ICommandContext.CommandId ), context.CommandId.ToString() },
                { CrsPrefix + nameof( ICommandContext.Monitor ), context.Monitor.DependentActivity().CreateToken().ToString() },

                // Rebus Headers
                //{ R.Messages.Headers.ReturnAddress, "" },
                { R.Messages.Headers.MessageId, context.CommandId.ToString() }
            };
        }

        public static Dictionary<string, string> CreateResultHeaders( this ICommandContext context )
        {
            return new Dictionary<string, string>
            {
                // CRS Headers
                { CrsPrefix + "CommandName", context.Model.Name },
                { CrsPrefix + "IsResult", "true" },
                { CrsPrefix + nameof( ICommandContext.CallerId ), context.CallerId },
                { CrsPrefix + nameof( ICommandContext.CommandId ), context.CommandId.ToString() },
                { CrsPrefix + nameof( ICommandContext.Monitor ), context.Monitor.DependentActivity().CreateToken().ToString() },

                // Rebus Headers
                //{ R.Messages.Headers.ReturnAddress, "" },
                { R.Messages.Headers.MessageId, context.CommandId.ToString() }
            };
        }

        /// <summary>
        /// Gets an <see cref="IActivityMonitor"/> from <see cref="IMessageContext"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IActivityMonitor GetActivityMonitor( this IMessageContext context )
        {
            var key = CrsPrefix + nameof( ICommandContext.Monitor );
            return context.TransactionContext.GetOrAdd( key, () =>
            {
                var monitor = new ActivityMonitor();
                var monitorToken = context.Headers[key];
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
            var key = CrsPrefix + nameof( ICommandContext.CommandId );
            return context.TransactionContext.GetOrAdd( key, () =>
            {
                var commandId = context.Headers[key];
                if( commandId != null )
                {
                    return Guid.Parse( commandId );
                }
                return Guid.Empty;
            } );
        }

        public static string GetCallerId( this IMessageContext context )
        {
            var key = CrsPrefix + nameof( ICommandContext.CallerId );
            return context.TransactionContext.GetOrAdd( key, () => context.Headers[key] );
        }

        public static string GetCommandName( this IMessageContext context )
        {
            var key = CrsPrefix + "CommandName";
            return context.TransactionContext.GetOrAdd( key, () => context.Headers[key] );
        }
    }
}
