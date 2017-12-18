using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotKit.RESTutils;

namespace DotKit.RESTserver.Tests
{
    public sealed class TestWebServer : WebServer
    {
        public TestWebServer()
        {
            AuthorizationVerifier = new TestAuthorizationVerifier();
            this.RegisterRequestProcessor(typeof(TestGETRequestProcessor));
            this.RegisterRequestProcessor(typeof(TestPOSTRequestProcessor));
            this.RegisterRequestProcessor(typeof(TestDELETERequestProcessor));
        }
    }
}
