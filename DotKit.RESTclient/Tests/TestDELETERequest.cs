using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotKit.RESTutils;
using DotKit.RESTutils.Tests;

namespace DotKit.RESTclient.Tests
{
    public sealed class TestDELETERequest: ADELETERequest<TestResponse>
    {
        public TestDELETERequest(RequestSettings settings)
            : base(settings)
        {
        }

        public bool Process()
        {
            var req = new TestRequest();
            var response = Execute("/test", req);
            return response != null &&
                response.Msg == req.Msg &&
                response.Number == req.Number &&
                response.Time == req.Time;
        }
}
}
