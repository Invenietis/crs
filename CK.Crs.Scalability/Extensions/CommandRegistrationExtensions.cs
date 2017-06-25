using CK.Core;
using CK.Crs.Scalability;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{

    public static class CommandRegistrationExtensions
    {
        /// <summary>
        /// Makes the command scalable
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static ICommandRegistration IsScalable( this ICommandRegistration @this )
        {
            var t = @this.Traits.FindOrCreate( Traits.Scalable );
            if (@this.Description.Traits == null) @this.Description.Traits = t;
            else @this.Description.Traits = @this.Description.Traits.Union(t);

            return @this;
        }
    }
}
