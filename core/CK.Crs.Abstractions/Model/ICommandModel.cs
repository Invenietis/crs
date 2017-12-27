using System;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandModel : IFilterable, IBindable
    {
        CommandName Name { get; }
        Type CommandType { get; }
        string Description { get; }
        Type HandlerType { get; }
        Type ResultType { get; }
        CKTrait Tags { get; set; }
    }
}
