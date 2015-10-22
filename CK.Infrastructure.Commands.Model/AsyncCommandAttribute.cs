using System;

namespace CK.Infrastructure.Commands
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
    public class AsyncCommandAttribute : Attribute
    {
    }
}
