using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Samples.Messages
{
    public class SuperEvent : MessageBase
    {
        public SuperEvent( string id, int actorId, int authenticatedActorId )
        {
            Id = id;
            ActorId = actorId;
            AuthenticatedActorId = authenticatedActorId;
        }

        public string Message { get; set; }
        public string Id { get; private set; }
    }
}
