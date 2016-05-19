using System;
using CK.Core;

namespace CK.Crs
{
    public class Event
    {
        public object Payload { get; set; }

        public string EventType { get; set; }

        public ActivityMonitor.DependentToken Token { get; set; }

        public Event( ActivityMonitor.DependentToken token, object @event, Type evtType )
        {
            Token = token;
            Payload = @event;
            EventType = evtType.Name;
        }
    }
}