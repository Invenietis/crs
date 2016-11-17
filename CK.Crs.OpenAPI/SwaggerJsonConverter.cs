using System;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CK.Crs.OpenAPI
{
    class SwaggerJsonConverter : JsonNetConverter
    {
        JsonSerializerSettings _converterSettings;

        public override string ToJson( CommandResponse o )
        {
            return JsonConvert.SerializeObject( o.Payload, ConverterSettings );
        }

        public override JsonSerializerSettings ConverterSettings
        {
            get
            {
                if( _converterSettings == null )
                    _converterSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = base.ConverterSettings.Formatting,
                        ContractResolver = new SwaggerContractResolver( base.ConverterSettings )
                    };

                return _converterSettings;
            }
        }
        class SwaggerContractResolver : DefaultContractResolver
        {
            private readonly JsonSerializer _appJsonSerializer;
            private readonly CamelCasePropertyNamesContractResolver _camelCasePropertyNamesContractResolver;

            public SwaggerContractResolver( JsonSerializerSettings appSerializerSettings )
            {
                _appJsonSerializer = JsonSerializer.Create( appSerializerSettings );
                _camelCasePropertyNamesContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            public override JsonContract ResolveContract( Type type )
            {
                var defaultContract = base.ResolveContract(type);
                if( defaultContract is JsonDictionaryContract ) return defaultContract;

                return _camelCasePropertyNamesContractResolver.ResolveContract( type );
            }
        }
    }
}
