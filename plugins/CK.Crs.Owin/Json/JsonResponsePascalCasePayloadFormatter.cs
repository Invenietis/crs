using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace CK.Crs.Owin
{
    public class JsonResponsePascalCasePayloadFormatter : IResponseFormatter
    {
        private JsonSerializerSettings _defaultSettings;
        private readonly JsonSerializerSettings _pascalSettings;

        public JsonResponsePascalCasePayloadFormatter( JsonSerializerSettings settings )
        {
            _defaultSettings = settings;
            _pascalSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
            };
        }

        public string ContentType => "application/json";

        public string Format( Response response )
        {
            // Meta responses always use camelCase
            if( response.ResponseType == 'M' )
            {
                return JsonConvert.SerializeObject( response, _defaultSettings );
            }

            // Payload responses (Response<T>) serialized their root in camelCase, and the payload in PascalCase
            object payload = response.GetType().GetProperty( "Payload" )?.GetValue( response );
            if( payload != null )
            {
                string serializedPayloadString = JsonConvert.SerializeObject( payload, _pascalSettings );

                return string.Format(
                    @"{{""commandId"":""{0}"",""responseType"":""{1}"",""payload"":{2}}}",
                    response.CommandId,
                    response.ResponseType.ToString(),
                    serializedPayloadString
                    );
            }


            return JsonConvert.SerializeObject( response, _defaultSettings );
        }
    }
}
