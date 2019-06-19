using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Responses
{
    public static class ResponseUtil
    {
        internal static Response CreateGenericResponse( object result, ICommandContext context )
        {
            if( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }

            if( context.Model.ResultType == null )
            {
                return new Response<object>( context.CommandId );
            }

            var genericResponseType = typeof( Response<> ).MakeGenericType( context.Model.ResultType );
            var response = (Response)Activator.CreateInstance( genericResponseType, context.CommandId, result );
            return response;
        }
    }
}
