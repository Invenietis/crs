﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandValidator
    {
        bool TryValidate( CommandExecutionContext executionContext, out ICollection<ValidationResult> results );
    }
}