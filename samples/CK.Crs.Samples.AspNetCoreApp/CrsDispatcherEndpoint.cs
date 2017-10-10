using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [NoAmbientValuesValidation]
    [Route( "crs-dispatcher" )]
    public class CrsDispatcherEndpoint<T> : HttpEndpoint<T>
    {
    }

}
