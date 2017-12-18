using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotKit.RESTutils;
using DotKit.RESTutils.Tests;

namespace DotKit.RESTserver.Tests
{
    public sealed class TestDELETERequestProcessor : ADELETERequestProcessor
    {
        public TestDELETERequestProcessor(WebServer webServer)
            : base(webServer)
        {
        }

        protected override object Execute(IDictionary<string, string> parameters, string path, string userName)
        {
            var msg = parameters["Msg"];
            var number = int.Parse(parameters["Number"]);
            var time = DataEncoder.Default.DateFromString(parameters["Time"]);
            return new TestResponse()
            {
                Msg = msg,
                Number = number,
                Time = time
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
