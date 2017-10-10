using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    [AttributeUsage( AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class NoAmbientValuesValidationAttribute : Attribute, IFilterMetadata
    {
    }
}
