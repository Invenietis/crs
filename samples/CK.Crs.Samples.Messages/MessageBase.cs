using Paramore.Brighter;
using System;

namespace CK.Crs.Samples.Messages
{
    public abstract class MessageBase : IRequest
    {
        public MessageBase()
        {
            Id = Guid.NewGuid();
        }

        public int ActorId { get; set; }
        public int AuthenticatedActorId { get; set; }
        public Guid Id { get; set; }
    }

}
