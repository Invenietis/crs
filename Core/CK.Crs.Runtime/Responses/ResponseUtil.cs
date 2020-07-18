using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Responses
{
    public static class ResponseUtil
    {
        /// <summary>
        /// Creates a generic <see cref="Response"/> from the given command result and <paramref name="context"/>,
        /// using the type parameter from <see cref="ICommandModel.ResultType"/>.
        /// </summary>
        /// <param name="result">The command result added to the <see cref="Response"/>.</param>
        /// <param name="context">The command context and metadata.</param>
        /// <returns>The parameterized <see cref="Response"/>.</returns>
        public static Response CreateGenericResponse( object result, ICommandContext context )
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
