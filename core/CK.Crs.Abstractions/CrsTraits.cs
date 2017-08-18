using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class CrsTraits
    {
        public static readonly string Queue = "Queue";

        public static bool HasQueueTag( this CommandModel commandModel )
        {
            string traits = CrsTraits.Queue;
            CKTrait rebusTrait = commandModel.Tags.Context.FindOrCreate( traits );
            return commandModel.Tags.Overlaps( rebusTrait );
        }

        public static ICommandRegistration IsQueue( this ICommandRegistration commandRegistration )
        {
            string traits = CrsTraits.Queue;
            CKTrait rebusTrait = commandRegistration.Model.Tags.Context.FindOrCreate( traits );
            commandRegistration.Model.Tags = commandRegistration.Model.Tags.Apply( rebusTrait, SetOperation.Union );
            return commandRegistration;
        }
    }
}
