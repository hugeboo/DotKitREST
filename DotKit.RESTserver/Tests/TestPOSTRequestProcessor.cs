using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotKit.RESTutils;
using DotKit.RESTutils.Tests;

namespace DotKit.RESTserver.Tests
{
    public sealed class TestPOSTRequestProcessor : APOSTRequestProcessor<TestRequest>
    {
        public TestPOSTRequestProcessor(WebServer webServer)
            : base(webServer)
        {
        }

        protected override object Execute(TestRequest request, IDictionary<string, string> parameters, string path, string userName)
        {
            return new TestResponse()
            {
                Msg = request.Msg,
                Number = request.Number,
                Time = request.Time
            };
        }

        public override string GetPathForListener()
        {
            return "/test";
        }

        protected override bool CheckPath(string path)
        {
            return "/test".Equals(path);
        }
    }
}
