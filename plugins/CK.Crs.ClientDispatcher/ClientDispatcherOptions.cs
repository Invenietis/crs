using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public class ClientDispatcherOptions
    {
        public string WebSocketPath { get; set; } = "/ws";
        public bool SupportsServerSideEventsFiltering { get; set; } = false;
    }
}
