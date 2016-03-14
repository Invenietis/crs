using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CK.Crs
{
    public static class CommandDescriptionExtensions
    {
        const string AuthorizationKey = "Microsoft.AspNetCore.Authorization.IAuthorizationRequirement";

        public static void AddAuthorizationRequirement( this CommandDescription desc, IAuthorizationRequirement requirement )
        {
            object requirements;
            desc.ExtraData.TryGetValue( AuthorizationKey, out requirements );
            if( requirements == null )
            {
                desc.ExtraData.Add( AuthorizationKey, new List<IAuthorizationRequirement>( new[] { requirement } ) );
            }
            else
            {
                (requirements as List<IAuthorizationRequirement>)?.Add( requirement );
            }
        }

        public static IReadOnlyCollection<IAuthorizationRequirement> GetAuthorizationRequirement( this CommandDescription desc )
        {
            object requirements;
            if( desc.ExtraData.TryGetValue( AuthorizationKey, out requirements ) )
            {
                return requirements as List<IAuthorizationRequirement>;
            }
            return CK.Core.Util.Array.Empty<IAuthorizationRequirement>();
        }
    }
}
