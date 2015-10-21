using System;

namespace CK.Infrastructure.Commands.Framework
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
    public class AsyncCommandAttribute : Attribute
    {
    }


}
