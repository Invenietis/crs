using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.Tests.Fake
{

    internal class FakeHttpRequest : HttpRequest
    {
        HttpContext _context;
        public FakeHttpRequest( HttpContext context )
        {
            _context = context;
        }

        public override Stream Body { get; set; }

        public override long? ContentLength { get; set; }

        public override string ContentType { get; set; }

        public override IRequestCookieCollection Cookies { get; set; }

        public override IFormCollection Form { get; set; }

        public override bool HasFormContentType
        {
            get
            {
                return false;
            }
        }

        public override IHeaderDictionary Headers
        {
            get
            {
                return null;
            }
        }

        public override HostString Host { get; set; }

        public override HttpContext HttpContext
        {
            get
            {
                return _context;
            }
        }

        public override bool IsHttps { get; set; }

        public override string Method { get; set; }

        public override PathString Path { get; set; }

        public override PathString PathBase { get; set; } = "/";

        public override string Protocol { get; set; }

        public override IQueryCollection Query { get; set; }
        public override QueryString QueryString { get; set; }

        public override string Scheme { get; set; }

        public override Task<IFormCollection> ReadFormAsync( CancellationToken cancellationToken = default( CancellationToken ) )
        {
            throw new NotImplementedException();
        }
    }

}
