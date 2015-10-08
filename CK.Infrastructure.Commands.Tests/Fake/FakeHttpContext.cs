using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features;

namespace CK.Infrastructure.Commands.Tests.Fake
{

    internal class FakeHttpContext : HttpContext
    {
        IServiceProvider _sp;
        IServiceProvider _scopedSp;
        HttpRequest _request;

        public FakeHttpContext( IServiceProvider sp, string requestPath, Stream requestBody )
        {
            _sp = sp;
            _scopedSp = sp;

            _user = new ClaimsPrincipal();
            _request = CreateRequest( requestPath, requestBody );
        }

        private HttpRequest CreateRequest( string requestPath, Stream requestBody )
        {
            return new FakeHttpRequest( this )
            {
                Path = requestPath,
                Body = requestBody
            };
        }

        public override IServiceProvider ApplicationServices
        {
            get
            {
                return _sp;
            }

            set
            {
                _sp = value;
            }
        }

        public override AuthenticationManager Authentication
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ConnectionInfo Connection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IDictionary<object,object> _o = new Dictionary<object,object>();

        public override IDictionary<object, object> Items
        {
            get
            {
                return _o;
            }

            set
            {
                _o = value;
            }
        }

        public override HttpRequest Request
        {
            get
            {
                return _request;
            }
        }

        CancellationToken _rqAbortedToken = CancellationToken.None;
        public override CancellationToken RequestAborted
        {
            get
            {
                return _rqAbortedToken;
            }

            set
            {
                _rqAbortedToken = value;
            }
        }

        public override IServiceProvider RequestServices
        {
            get
            {
                return _scopedSp;
            }

            set
            {
                _scopedSp = value;
            }
        }

        public override HttpResponse Response
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ISession _session;
        public override ISession Session
        {
            get
            {
                return _session;
            }

            set
            {
                _session = value;
            }
        }

        ClaimsPrincipal _user;
        public override ClaimsPrincipal User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
            }
        }

        public override WebSocketManager WebSockets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Abort()
        {
        }

        public override object GetFeature( Type type )
        {
            return null;
        }

        public override void SetFeature( Type type, object instance )
        {
        }

        public override void Dispose()
        {
        }
    }
}
