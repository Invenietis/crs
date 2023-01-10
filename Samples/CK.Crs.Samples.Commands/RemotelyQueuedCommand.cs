using System;

namespace CK.Crs.Samples.Messages
{
    public class RemotelyQueuedCommand
    {
        public int ActorId { get; set; }
        public int AuthenticatedActorId { get; set; }

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
