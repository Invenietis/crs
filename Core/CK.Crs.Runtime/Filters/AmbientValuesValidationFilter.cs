using CK.Core;
using CK.Crs.Responses;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class AmbientValuesValidationFilter : ICommandFilter, ICommandSecurityFilter
    {
        public AmbientValuesValidationFilter( IAmbientValues ambientValues, IAmbientValuesRegistration ambientValuesRegistration )
        {
            AmbientValues = ambientValues;
            AmbientValuesRegistration = ambientValuesRegistration;
        }

        public IAmbientValues AmbientValues { get; }
        public IAmbientValuesRegistration AmbientValuesRegistration { get; }

        public async Task OnFilterAsync( CommandFilterContext filterContext )
        {
            using( filterContext.Monitor.OpenTrace( "Validating ambient values" ) )
            {
                var vContext = new ReflectionAmbientValueValidationContext( filterContext.Command, filterContext.Monitor, AmbientValues );

                foreach( var v in AmbientValuesRegistration.AmbientValues )
                {
                    await vContext.ValidateValueAndRejectOnError( v.Name );
                }

                if( vContext.Rejected )
                {
                    filterContext.SetResponse( new InvalidResponse( filterContext.CommandContext.CommandId, vContext.RejectReason ) );
                }
            }
        }
    }
}
