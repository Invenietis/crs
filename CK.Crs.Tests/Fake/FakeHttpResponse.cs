using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.Tests.Fake
{
    internal class FakeHttpResponse : HttpResponse
    {
        HttpContext _context;
        public FakeHttpResponse( HttpContext context )
        {
            _context = context;
            Body = new MemoryStream();
        }

        public override Stream Body { get; set; }

        public override long? ContentLength { get; set; } 

        public override string ContentType { get; set; } 

        public override IResponseCookies Cookies
        {
            get
            {
                return null;
            }
        }

        public override bool HasStarted
        {
            get
            {
                return true;
            }
        }

        public override IHeaderDictionary Headers
        {
            get
            {
                return null;
            }
        }

        public override HttpContext HttpContext
        {
            get
            {
                return _context;
            }
        }

        public override int StatusCode { get; set; }

        public override void OnCompleted( Func<object, Task> callback, object state )
        {
        }

        public override void OnStarting( Func<object, Task> callback, object state )
        {
        }

        public override void Redirect( string location, bool permanent )
        {
        }
    }
}