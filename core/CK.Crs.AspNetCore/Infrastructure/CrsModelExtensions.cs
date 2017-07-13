using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Infrastructure
{
    public static class CrsModelExtensions
    {
        public static ICrsReceiverModel GetEndpointFromContext( this ICrsModel model, ActionContext context )
        {
            string endpointName = context.RouteData.Values["Controller"].ToString();
            return model.Receivers.SingleOrDefault( e => e.Name == endpointName );
        }
    }
}
