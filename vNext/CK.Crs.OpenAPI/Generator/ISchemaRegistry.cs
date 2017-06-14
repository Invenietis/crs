using System;
using System.Collections.Generic;


namespace CK.Crs.OpenAPI.Generator
{
    public interface ISchemaRegistry
    {
        Schema GetOrRegister(Type type);

        IDictionary<string, Schema> Definitions { get; }
    }
}
