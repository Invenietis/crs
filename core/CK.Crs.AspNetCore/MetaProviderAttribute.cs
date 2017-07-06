using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using CK.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CK.Crs
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MetaProviderAttribute : TypeFilterAttribute
    {
        public MetaProviderAttribute() : base(typeof(CrsMetaProviderImpl)) { }

        class CrsMetaProviderImpl : IAsyncActionFilter
        {
            readonly IAmbientValues _ambientValues;
            readonly IAmbientValuesRegistration _registration;
            readonly IMemoryCache _cache;

            public CrsMetaProviderImpl(IAmbientValues ambientValues, IAmbientValuesRegistration registration, IMemoryCache cache )
            {
                _ambientValues = ambientValues;
                _registration = registration;
                _cache = cache;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!context.RouteData.Values.TryGetValue("action", out object actionTokenValue))
                {
                    await next();
                    return;
                }

                if (actionTokenValue == null || actionTokenValue.ToString() != "__meta")
                {
                    await next();
                    return;
                }

                var result = new MetaCommand.MetaResult();
                var commandArgumentName = context.ActionDescriptor.GetProperty<CrsCommandArgumentName>();
                var command = context.ActionArguments[commandArgumentName] as MetaCommand ?? new MetaCommand
                {
                    ShowAmbientValues = true,
                    ShowCommands = true
                };
                if (command.ShowAmbientValues)
                {
                    result.AmbientValues = new Dictionary<string, object>();
                    foreach (var a in _registration.AmbientValues)
                    {
                        if (_ambientValues.IsDefined(a.Name))
                        {
                            var o = await _ambientValues.GetValueAsync<object>(a.Name);
                            result.AmbientValues.Add(a.Name, o);
                        }
                    }
                }
                if (command.ShowCommands)
                {
                    result.Commands = new Dictionary<string, MetaCommand.MetaResult.MetaCommandDescription>();
                    //foreach (var c in _routes.All)
                    //{
                    //    MetaCommand.MetaResult.MetaCommandDescription desc;
                    //    if (!_cache.TryGetValue(c.Route.FullPath, out desc))
                    //    {
                    //        desc = new MetaCommand.MetaResult.MetaCommandDescription
                    //        {
                    //            Route = c.Route,
                    //            CommandType = c.Descriptor.CommandType.AssemblyQualifiedName,
                    //            Parameters = c.Descriptor.CommandType.GetTypeInfo().DeclaredProperties.Select(e => new CommandPropertyInfo(e, _registration)).ToArray(),
                    //            Traits = c.Descriptor.Traits.ToString(),
                    //            Description = c.Descriptor.Description
                    //        };
                    //        _cache.Set(c.Route.FullPath, desc);
                    //    }
                    //    result.Commands.Add(c.Route.CommandName, desc);
                    //}
                }
                context.Result = new OkObjectResult(new MetaCommandResponse(result));
            }
        }
    }
}
