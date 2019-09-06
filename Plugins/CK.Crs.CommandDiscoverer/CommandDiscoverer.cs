using System;
using System.Linq;
using System.Reflection;
using CK.Crs.CommandDiscoverer.Attributes;

namespace CK.Crs.CommandDiscoverer
{
    public static class CommandDiscoverer
    {
        public static ICommandRegistry RegisterHandlers( this ICommandRegistry @this, Assembly assembly )
        {
            var handlers = assembly
                .GetTypes()
                .Where( type => type.GetInterfaces().Contains( typeof( ICommandHandler ) ) )
                .Select( handler =>
                    new HandlerWithGenerics
                    (
                        handler,
                        handler
                            .GetInterfaces()
                            .Where( type => type.IsGenericType &&
                                            (type.GetGenericTypeDefinition() == typeof( ICommandHandler<> ) ||
                                             type.GetGenericTypeDefinition() == typeof( ICommandHandler<,> )) )
                            .Select( type => type.GetGenericArguments() )
                    )
                );

            foreach( var (handler, interfaces) in handlers )
                foreach( var types in interfaces )
                {
                    var command = types[0];

                    var registration = @this.Register( command, types.Length == 1 ? null : types[1] ).Handler( handler );

                    var broadcastResult = (CommandBroadcastResultAttribute)
                        Attribute.GetCustomAttribute( command, typeof( CommandBroadcastResultAttribute ) );
                    if( broadcastResult != null )
                        registration.BroadcastResult();

                    var customBinder = (CommandCustomBinderAttribute)
                        Attribute.GetCustomAttribute( command, typeof( CommandCustomBinderAttribute ) );
                    if( customBinder != null )
                        registration.CustomBinder( customBinder.CommandBinder );

                    var filters = (CommandFiltersAttribute)
                        Attribute.GetCustomAttribute( command, typeof( CommandFiltersAttribute ) );
                    if( filters != null )
                        registration.AddFilters( filters.Filters );

                    var fireAndForget = (CommandFireAndForgetAttribute)
                        Attribute.GetCustomAttribute( command, typeof( CommandFireAndForgetAttribute ) );
                    if( fireAndForget != null )
                        registration.FireAndForget();

                    var name = (CommandNameAttribute)
                        Attribute.GetCustomAttribute( command, typeof( CommandNameAttribute ) );
                    if( name != null )
                        registration.CommandName( name.Name );

                    var setResultTag = (CommandSetResultTagAttribute)
                        Attribute.GetCustomAttribute( command, typeof( CommandSetResultTagAttribute ) );
                    if( setResultTag != null )
                        registration.SetResultTag();

                    var setTag = (CommandSetTagAttribute)
                        Attribute.GetCustomAttribute( command, typeof( CommandSetTagAttribute ) );
                    if( setTag != null )
                        registration.SetTag( setTag.Tags );
                }

            return @this;
        }
    }
}
