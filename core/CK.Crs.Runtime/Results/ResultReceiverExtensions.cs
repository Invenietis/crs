using System;
using System.Threading.Tasks;

namespace CK.Crs.Results
{
    public static class ResultReceiverExtensions
    {
        public static Task InvokeGenericResult( this IResultReceiver resultReceiver, object result, ICommandContext context )
        {
            if( context.Model.ResultType == null )
                throw new InvalidOperationException( "Cannot invoke a generic result when there is no result typed assigned for the command." );

            var resultReceiveMethod = resultReceiver.GetType().GetMethod( nameof( IResultReceiver.ReceiveResult ) );
            resultReceiveMethod = resultReceiveMethod.MakeGenericMethod( context.Model.ResultType );
            return (Task)resultReceiveMethod.Invoke( resultReceiver, new[] { result, context } );
        }
    }
}
