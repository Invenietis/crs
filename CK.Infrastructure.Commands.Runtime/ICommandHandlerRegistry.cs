﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandHandlerRegistry
    {
        void RegisterHandler<T, THandler>();
        bool IsRegisterd( Type commandType );
        Type GetHandlerType( Type commandType );
    }

}
