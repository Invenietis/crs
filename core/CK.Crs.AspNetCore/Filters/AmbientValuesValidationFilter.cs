using CK.Core;
using CK.Crs.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore
{
    public class AmbientValuesValidationFilter : ICrsFilter
    {
        public async Task ApplyFilterAsync( CrsFilterContext filterContext )
        {
            if( filterContext.Model.ApplyAmbientValuesValidation )
            {
                using( filterContext.Monitor.OpenTrace( "Validating ambient values" ) )
                {
                    IAmbientValues ambientValues = filterContext.HttpContext.RequestServices.GetRequiredService<IAmbientValues>();
                    IAmbientValuesRegistration ambientValueRegistration = filterContext.HttpContext.RequestServices.GetRequiredService<IAmbientValuesRegistration>();
                    var vContext = new ReflectionAmbientValueValidationContext( filterContext.Command, filterContext.Monitor, ambientValues );

                    foreach( var v in ambientValueRegistration.AmbientValues )
                    {
                        await vContext.ValidateValueAndRejectOnError( v.Name );
                    }

                    if( vContext.Rejected )
                    {
                        filterContext.SetResponse( new InvalidResponse( filterContext.Context.CommandId, vContext.RejectReason ) );
                    }
                }
            }
            else filterContext.Monitor.Trace( "Ambient values validation skipped by endpoint configuration" );
        }
    }
}
