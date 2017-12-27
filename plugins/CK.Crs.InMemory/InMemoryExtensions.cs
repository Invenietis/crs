using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class InMemoryExtensions
    {
        public static readonly string InMemoryTag = "InMemory";

        public static bool HasFireAndForgetTag( this ICommandModel commandModel )
        {
            return commandModel.HasTags( InMemoryTag, CrsTraits.FireForget );
        }

        public static ICommandRegistration FireAndForget( this ICommandRegistration commandRegistration )
        {
            return commandRegistration.SetTag( InMemoryTag, CrsTraits.FireForget );
        }

    }
}
