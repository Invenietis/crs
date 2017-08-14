using CK.Core;
using System;
using Rebus.Logging;
using MessageTemplates.Parsing;

namespace CK.Crs.Rebus
{

    public class GrandOutputRebusLoggerFactory : AbstractRebusLoggerFactory
    {
        MessageTemplateParser _parser;
        GrandOutputExternalLogger _logger;

        public GrandOutputRebusLoggerFactory( Guid? monitorId = null, CKTraitContext context = null )
        {
            if( monitorId == null ) monitorId = Guid.NewGuid();
            if( context == null ) context = new CKTraitContext( "GrandOutput" );

            _parser = new MessageTemplateParser();
            _logger = new GrandOutputExternalLogger( monitorId.Value, _parser, context.FindOrCreate( "External" ) );
        }

        protected override ILog GetLogger( Type type )
        {
            return new RebusLoggerAdapter( GetTopicFor( type ), _logger );
        }

        private string GetTopicFor( Type type )
        {
            return type.AssemblyQualifiedName;
        }

        class RebusLoggerAdapter : ILog
        {
            GrandOutputExternalLogger _logger;
            string _topic;
            public RebusLoggerAdapter( string topic, GrandOutputExternalLogger logger )
            {
                _topic = topic;
                _logger = logger;
            }

            public void Debug( string message, params object[] objs )
            {
                _logger.Log( Core.LogLevel.Debug, _topic, message, null, objs );
            }

            public void Error( string message, params object[] objs )
            {
                _logger.Log( Core.LogLevel.Error, _topic, message, null, objs );
            }

            public void Error( Exception exception, string message, params object[] objs )
            {
                _logger.Log( Core.LogLevel.Error, _topic, message, exception, objs );
            }

            public void Info( string message, params object[] objs )
            {
                _logger.Log( Core.LogLevel.Info, _topic, message, null, objs );
            }

            public void Warn( string message, params object[] objs )
            {
                _logger.Log( Core.LogLevel.Warn, _topic, message, null, objs );
            }

            public void Warn( Exception exception, string message, params object[] objs )
            {
                _logger.Log( Core.LogLevel.Warn, _topic, message, exception, objs );
            }
        }
    }
}
