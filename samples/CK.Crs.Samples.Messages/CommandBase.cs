using Paramore.Brighter;
using System;

namespace CK.Crs.Samples.Messages
{
    public abstract class CommandBase : Command
    {
        public CommandBase() : base(Guid.NewGuid())
        {
        }

        public int ActorId { get; set; }
        public int AuthenticatedActorId { get; set; }
    }

}
