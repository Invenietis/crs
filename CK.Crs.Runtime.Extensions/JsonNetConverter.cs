using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CK.Crs.Runtime
{
    public class JsonNetConverter : IJsonConverter
    {
        readonly JsonSerializerSettings _converterSettings;
        public JsonNetConverter()
        {
            _converterSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public string ToJson( CommandResponse o )
        {
            return JsonConvert.SerializeObject( o, _converterSettings );
        }

        public object ParseJson( string json, Type type )
        {
            return JsonConvert.DeserializeObject( json, type, _converterSettings );
        }
    }
}
