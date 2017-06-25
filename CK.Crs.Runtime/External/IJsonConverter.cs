﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public interface IJsonConverter
    {
        string ToJson( object payload );

        object ParseJson( string json, Type type );
    }
}
