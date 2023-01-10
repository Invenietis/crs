using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class CrsTags
    {
        public static readonly string FireForget = "FireForget";
        public static readonly string Result = "Result";
        public static readonly string Broadcast = "Broadcast";

        public static bool HasResultBroadcastTag( this ICommandModel commandModel )
        {
            return HasTag( commandModel, CrsTags.Broadcast );
        }

        public static ICommandRegistration BroadcastResult( this ICommandRegistration commandRegistration )
        {
            return SetTag( commandRegistration, CrsTags.Result, CrsTags.Broadcast );
        }

        public static bool HasResultTag( this ICommandModel commandModel )
        {
            return HasTag( commandModel, CrsTags.Result );
        }

        public static ICommandRegistration SetResultTag( this ICommandRegistration commandRegistration )
        {
            return SetTag( commandRegistration, CrsTags.Result );
        }

        public static bool HasTag( this ICommandModel commandModel, string tag )
        {
            return commandModel.Tags.Overlaps( commandModel.Tags.Context.FindOrCreate( tag ) );
        }

        public static bool HasTags( this ICommandModel commandModel, params string[] tags )
        {
            return commandModel.Tags.HasTags( tags );
        }
        public static bool HasTags( this CKTrait @this, params string[] tags )
        {
            var combinedTags = String.Join( @this.Context.Separator.ToString(), tags );
            CKTrait tag = @this.Context.FindOrCreate( combinedTags );
            return @this.IsSupersetOf( tag );
        }

        public static ICommandRegistration SetTag( this ICommandRegistration commandRegistration, params string[] tags )
        {
            return commandRegistration.SetTag( String.Join( commandRegistration.Model.Tags.Context.Separator.ToString(), tags ) );
        }
        
        public static ICommandRegistration SetTag( this ICommandRegistration commandRegistration, string tags )
        {
            CKTrait tag = commandRegistration.Model.Tags.Context.FindOrCreate( tags );
            commandRegistration.Model.Tags = commandRegistration.Model.Tags.Apply( tag, SetOperation.Union );
            return commandRegistration;
        }

    }
}
