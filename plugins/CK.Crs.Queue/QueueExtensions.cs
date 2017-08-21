using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class QueueExtensions
    {
        public static readonly string InProcessTag = "InProcess";

        public static bool HasInProcessQueueTag( this CommandModel commandModel )
        {
            return commandModel.HasTags( InProcessTag, CrsTraits.Queue );
        }

        public static ICommandRegistration IsInProcessQueue( this ICommandRegistration commandRegistration )
        {
            return commandRegistration.SetTag( InProcessTag, CrsTraits.Queue );
        }

    }
}
