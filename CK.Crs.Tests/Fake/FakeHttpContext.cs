using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.Primitives;

namespace CK.Crs.Tests.Fake
{

    internal class FakeHttpContext : HttpContext, IDisposable
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

        public override string TraceIdentifier { get; set; }

        private HttpRequest CreateRequest( string requestPath, Stream requestBody )
        {
            return new FakeHttpRequest( this )
            {
                Path = requestPath,
                Body = requestBody,
                Query = new ReadableStringCollection(  "c", "3712-451235-aze"  )
            };
        }

        class ReadableStringCollection : IReadableStringCollection
        {
            private string c; private string v;

            public ReadableStringCollection( string c, string v )
            {
                this.c = c;
                this.v = v;
            }

            public StringValues this[string key]
            {
                get
                {
                    return v;
                }
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool ContainsKey( string key )
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
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

        FakeHttpResponse _response;
        public override HttpResponse Response
        {
            get
            {
                return _response ?? (_response = new FakeHttpResponse( this ));
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

        public override IFeatureCollection Features
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Abort()
        {
        }

        public void Dispose()
        {
            Request.Body.Dispose();
            Response.Body.Dispose();
        }
    }
}
