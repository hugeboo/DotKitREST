using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotKit.RESTutils.Tests
{
    public sealed class TestRequest
    {
        public string Msg { get; set; }
        public int Number { get; set; }
        public DateTime Time { get; set; }

        public TestRequest()
        {
            Msg = "Test_Request_Тест";
            Number = 1234;
            Time = new DateTime(2015, 04, 28, 17, 25, 48, 666);
        }
    }
}
