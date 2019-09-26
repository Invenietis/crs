using CK.Monitoring;
using System;
using System.Collections.Generic;
using System.Text;
using CK.Core;
using System.IO;

namespace CK.Crs.Rebus.Tests
{
    public sealed class TextWriterHandlerConfiguration : IHandlerConfiguration
    {
        public TextWriter Out { get; set; }

        public IHandlerConfiguration Clone()
        {
            return new TextWriterHandlerConfiguration
            {
                Out = Out
            };
        }
    }

    public class TextWriterHandler : IGrandOutputHandler
    {
        TextWriterHandlerConfiguration _conf;

        public TextWriterHandler( TextWriterHandlerConfiguration conf )
        {
            _conf = conf;
        }

        public bool Activate( IActivityMonitor m )
        {
            return true;
        }

        public bool ApplyConfiguration( IActivityMonitor m, IHandlerConfiguration c )
        {
            _conf = c as TextWriterHandlerConfiguration;
            return true;
        }

        public void Deactivate( IActivityMonitor m )
        {
        }

        public void Handle( IActivityMonitor m, GrandOutputEventInfo logEvent )
        {
            _conf.Out.WriteLine( logEvent.Entry.Text );
        }

        public void OnTimer( IActivityMonitor m, TimeSpan timerSpan )
        {
        }
    }
}
