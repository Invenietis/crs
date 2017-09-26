using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class CrsTraits
    {
        public static readonly string FireForget = "FireForget";
        public static readonly string Result = "Result";
        public static readonly string Broadcast = "Broadcast";

        public static bool HasResultBroadcastTag( this CommandModel commandModel )
        {
            return HasTag( commandModel, CrsTraits.Broadcast );
        }

        public static ICommandRegistration BroadcastResult( this ICommandRegistration commandRegistration )
        {
            return SetTag( commandRegistration, CrsTraits.Result, CrsTraits.Broadcast );
        }

        public static bool HasResultTag( this CommandModel commandModel )
        {
            return HasTag( commandModel, CrsTraits.Result );
        }

        public static ICommandRegistration IsResultTag( this ICommandRegistration commandRegistration )
        {
            return SetTag( commandRegistration, CrsTraits.Result );
        }

        public static bool HasTag( this CommandModel commandModel, string trait )
        {
            return commandModel.Tags.Overlaps( commandModel.Tags.Context.FindOrCreate( trait ) );
        }

        public static bool HasTags( this CommandModel commandModel, params string[] traits )
        {
            return commandModel.Tags.HasTags( traits );
        }
        public static bool HasTags( this CKTrait tags, params string[] traits )
        {
            var combinedTraits = String.Join( tags.Context.Separator.ToString(), traits );
            CKTrait trait = tags.Context.FindOrCreate( combinedTraits );
            return tags.IsSupersetOf( trait );
        }

        public static ICommandRegistration SetTag( this ICommandRegistration commandRegistration, string traits )
        {
            CKTrait trait = commandRegistration.Model.Tags.Context.FindOrCreate( traits );
            commandRegistration.Model.Tags = commandRegistration.Model.Tags.Apply( trait, SetOperation.Union );
            return commandRegistration;
        }

        public static ICommandRegistration SetTag( this ICommandRegistration commandRegistration, params string[] traits )
        {
            return commandRegistration.SetTag( String.Join( commandRegistration.Model.Tags.Context.Separator.ToString(), traits ) );
        }
    }
}
