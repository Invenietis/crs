namespace CK.Crs.Samples.Messages
{
    public class SuperCommand : MessageBase
    {
        public class Result
        {
            public string Message { get; set; }

            public Result( string v )
            {
                Message = v;
            }
        }
    }
}
