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

        public virtual JsonSerializerSettings ConverterSettings
        {
            get
            {
                return _converterSettings;
            }
        }

        public JsonNetConverter()
        {
            _converterSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public virtual string ToJson( CommandResponse o )
        {
            return JsonConvert.SerializeObject( new
            {
                CommandId = o.CommandId,
                Payload = o.Payload,
                ResponseType = o.ResponseType
            }, ConverterSettings );
        }

        public virtual object ParseJson( string json, Type type )
        {
            return JsonConvert.DeserializeObject( json, type, ConverterSettings );
        }
    }
}
