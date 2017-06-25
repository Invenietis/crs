using CK.Core;
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
            @this.Description.Traits = "Scalable";
            return @this;
        }
    }
}
