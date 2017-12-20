using Newtonsoft.Json;

namespace CK.Crs
{
    public class JsonResponseFormatter : IResponseFormatter
    {
        private JsonSerializerSettings _settings;

        public JsonResponseFormatter( JsonSerializerSettings settings )
        {
            _settings = settings;
        }

        public string ContentType => "application/json";

        public string Format( Response response )
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject( response, _settings );
        }
    }
}
