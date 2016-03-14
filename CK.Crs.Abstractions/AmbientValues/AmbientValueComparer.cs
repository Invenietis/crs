using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public delegate bool AmbientValueComparer<T>( string valueName, T value, T ambientValue );
}
