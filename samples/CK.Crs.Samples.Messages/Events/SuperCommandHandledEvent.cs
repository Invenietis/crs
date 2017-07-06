using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Samples.Messages
{
    public class SuperCommandHandledEvent : MessageBase
    {
        public SuperCommandHandledEvent(Guid id, int actorId, int authenticatedActorId)
        {
            Id = id;
            ActorId = actorId;
            AuthenticatedActorId = authenticatedActorId;
        }
    }
}
