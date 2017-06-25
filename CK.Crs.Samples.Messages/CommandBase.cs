using System;

namespace CK.Crs.Samples.Messages
{
    public abstract class CommandBase
    {
        public int ActorId { get; set; }
        public int AuthenticatedActorId { get; set; }
    }

}
