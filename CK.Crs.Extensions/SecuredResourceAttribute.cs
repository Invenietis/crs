using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class SecuredResourceAttribute : Attribute
    {
        public SecuredResourceAttribute( string resourceType )
        {
            ResourceType = resourceType;
        }

        public string ResourceType { get; internal set; }
    }

}
