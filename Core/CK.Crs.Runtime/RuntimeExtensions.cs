using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class RuntimeExtensions
    {
        public static readonly string FireAndForgetTag = "FireAndForget";

        public static bool HasFireAndForgetTag( this ICommandModel commandModel )
        {
            return commandModel.HasTags( FireAndForgetTag, CrsTags.FireForget );
        }

        public static ICommandRegistration FireAndForget( this ICommandRegistration commandRegistration )
        {
            return commandRegistration.SetTag( FireAndForgetTag, CrsTags.FireForget );
        }

    }
}
