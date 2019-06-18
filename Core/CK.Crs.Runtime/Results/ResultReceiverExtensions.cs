using CK.Core;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Results
{
    public static class ResultReceiverExtensions
    {
        public static Task InvokeGenericResult( this IResultReceiver resultReceiver, object result, ICommandContext context )
        {
            Type resultType = context.Model.ResultType;

            if( resultType == null )
            {
                context.Monitor.Trace( $"Returning null result - command has no typed result" );
                resultType = typeof( object );
            }

            var resultReceiveMethod = resultReceiver.GetType().GetMethod( nameof( IResultReceiver.ReceiveResult ) );
            resultReceiveMethod = resultReceiveMethod.MakeGenericMethod( resultType );
            return (Task)resultReceiveMethod.Invoke( resultReceiver, new[] { result, context } );
        }
    }
}
