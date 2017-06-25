namespace CK.Crs.Scalability.Redis
{
    public class RedisSubscriberConfiguration
    {
        public string ConnectionString { get; set; }
        public bool UseEncryption { get; set; }
        public string ChannelName { get; set; }
    }
}