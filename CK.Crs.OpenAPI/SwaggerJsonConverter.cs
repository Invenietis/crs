using System.Threading.Tasks;
using CK.Crs.Runtime;
using Newtonsoft.Json;

namespace CK.Crs.OpenAPI
{
    class SwaggerJsonConverter : JsonNetConverter
    {
        public override string ToJson( CommandResponse o )
        {
            return JsonConvert.SerializeObject( o.Payload, ConverterSettings );
        }
    }
}
