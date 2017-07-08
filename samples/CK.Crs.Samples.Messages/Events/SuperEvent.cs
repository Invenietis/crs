using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Samples.Messages
{
    public class SuperEvent : MessageBase, IEvent
    {
        public SuperEvent(Guid id, int actorId, int authenticatedActorId)
        {
            Id = id;
            ActorId = actorId;
            AuthenticatedActorId = authenticatedActorId;
        }

        public string Message { get; set; }
    }
}
