using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Core
{
    public delegate bool AmbientValueComparer( string valueName, IComparable commandValue, IComparable ambientValue );
}
