namespace CK.Crs
{
    public class JsonResponseFormatter : IResponseFormatter
    {
        public string ContentType => "application/json";

        public string Format( Response response )
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject( response );
        }
    }
}
