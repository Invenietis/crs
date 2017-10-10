using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore.Tests
{
    [Route( "crs" )]
    [NoAmbientValuesValidation]
    [ValidateModel]
    class TestEndpoint<T> : HttpEndpoint<T> where T : class
    {
    }
}
