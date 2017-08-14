using System;
using MessageTemplates.Parsing;
using System.Globalization;
using CK.Monitoring;

namespace CK.Core
{

    public class GrandOutputExternalLogger
    {
        private MessageTemplateParser _parser;
        private Guid _monitorId;
        private CKTrait _tags;

        public GrandOutputExternalLogger( Guid monitorId, MessageTemplateParser parser, CKTrait tags )
        {
            _parser = parser;
            _monitorId = monitorId;
            _tags = tags;
        }

        static object _lock = new object();
        static DateTimeStamp _lastLogTime;

        public void Log( Core.LogLevel level, string topic, string message, Exception ex, params object[] objs )
        {
            if( GrandOutput.Default == null )
                throw new InvalidOperationException( "You must initializes the GrandOutput before using the GrandOutputExternalLogger" );

            // TODO: log the raw message with objs. Properties

            string parsedMessage = Format( message, objs );
            var logParsed = CreateMulticastLogEntry( Core.LogLevel.Debug, parsedMessage, null );
            GrandOutput.Default.Sink.Handle( new GrandOutputEventInfo( logParsed, topic ) );
        }


        private string Format( string message, params object[] objs )
        {
            return _parser.Parse( message ).Format( CultureInfo.CurrentCulture, objs );
        }

        private IMulticastLogEntry CreateMulticastLogEntry( Core.LogLevel level, string message, Exception ex )
        {
            DateTimeStamp lastLogTime;
            lock( _lock )
            {
                lastLogTime = _lastLogTime;
                _lastLogTime = new DateTimeStamp( _lastLogTime, DateTime.UtcNow );
            }

            var exceptionData = CKExceptionData.CreateFrom( ex );
            return LogEntry.CreateMulticastLog(
                _monitorId,
                LogEntryType.Line,
                lastLogTime,
                0,
                message,
                _lastLogTime,
                level,
                null,
                0,
                _tags,
                ex != null ? exceptionData : null );
        }
    }
}
