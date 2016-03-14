﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{

    public interface IAmbientValueProvider
    {
        /// <summary>
        /// Gets the value of an ambient parameter. 
        /// </summary>
        /// <param name="values">A reference to the all <see cref="IAmbientValues"/></param>
        /// <returns></returns>
        Task<object> GetValueAsync( IAmbientValues values );
    }
}
