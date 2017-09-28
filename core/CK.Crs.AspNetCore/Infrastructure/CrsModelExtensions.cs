using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Infrastructure
{
    public static class CrsModelExtensions
    {
        public static IEndpointModel GetEndpointFromContext( this ICrsModel model, ActionContext context )
        {
            string endpointName = context.RouteData.Values["Controller"].ToString();
            return model.Endpoints.SingleOrDefault( e => e.Name == endpointName );
        }
    }
}
