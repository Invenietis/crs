using CK.Core;
using System;
using Rebus.Logging;
using MessageTemplates.Parsing;
using CK.Monitoring;
using System.Globalization;

namespace CK.Crs.Rebus
{

    public class GrandOutputRebusLoggerFactory : AbstractRebusLoggerFactory
    {
        MessageTemplateParser _parser;
        GrandOutput _logger;

        public GrandOutputRebusLoggerFactory( GrandOutput grandOutput )
        {
            _parser = new MessageTemplateParser();
            _logger = grandOutput;// new GrandOutputExternalLogger( monitorId.Value, _parser, context.FindOrCreate( "External" ) );
        }

        protected override ILog GetLogger( Type type )
        {
            return new RebusLoggerAdapter( _logger, _parser );
        }

        private string GetTopicFor( Type type )
        {
            return type.AssemblyQualifiedName;
        }

        class RebusLoggerAdapter : ILog
        {
            MessageTemplateParser _parser;
            GrandOutput _logger;
       
            public RebusLoggerAdapter( GrandOutput logger, MessageTemplateParser parser )
            {
                _logger = logger;
                _parser = parser;
            }
            private string Format( string message, params object[] objs )
            {
                return _parser.Parse( message ).Format( CultureInfo.CurrentCulture, objs );
            }

            public void Debug( string message, params object[] objs )
            {
                string parsedMessage = Format( message, objs );
                _logger.ExternalLog( Core.LogLevel.Debug,  parsedMessage );
            }

            public void Error( string message, params object[] objs )
            {
                string parsedMessage = Format( message, objs );
                _logger.ExternalLog( Core.LogLevel.Error, parsedMessage );
            }

            public void Error( Exception exception, string message, params object[] objs )
            {
                string parsedMessage = Format( message, objs );
                _logger.ExternalLog( Core.LogLevel.Error, parsedMessage, exception );
            }

            public void Info( string message, params object[] objs )
            {
                string parsedMessage = Format( message, objs );
                _logger.ExternalLog( Core.LogLevel.Info, parsedMessage );
            }

            public void Warn( string message, params object[] objs )
            {
                string parsedMessage = Format( message, objs );
                _logger.ExternalLog( Core.LogLevel.Warn, parsedMessage );
            }

            public void Warn( Exception exception, string message, params object[] objs )
            {
                string parsedMessage = Format( message, objs );
                _logger.ExternalLog( Core.LogLevel.Warn, parsedMessage, exception );
            }
        }
    }
}
