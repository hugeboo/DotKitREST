using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotKit.RESTclient;
using DotKit.RESTclient.Tests;
using DotKit.RESTserver;
using DotKit.RESTserver.Tests;

namespace DotKitREST
{
    public static class HttpsTest
    {
        public static void SendGET()
        {
            var ss = new WebServerSettings()
            {
                Port = 9090,
                UseSSL = false,
                UseBasicAuthorization = true
            };
            var rs = new RequestSettings()
            {
                ServerAddress = "localhost:9090",
                TimeoutMSec = 5000,
                UseSSL = false,
                UseBasicAuthorization = true,
                UserId = "test",
                Password = "1234"
            };
            using (var server = new TestWebServer())
            {
                server.Settings = ss;
                server.Start();

                var req = new TestGETRequest(rs);
                var ok = req.Process();
                if (!ok) throw new ApplicationException("SendGET() failed. LastError=" + req.LastError);
            }
        }

        public static void SendPOST()
        {
            var ss = new WebServerSettings()
            {
                Port = 9090,
                UseSSL = false,
                UseBasicAuthorization = true
            };
            var rs = new RequestSettings()
            {
                ServerAddress = "localhost:9090",
                TimeoutMSec = 5000,
                UseSSL = false,
                UseBasicAuthorization = true,
                UserId = "test",
                Password = "1234"
            };
            using (var server = new TestWebServer())
            {
                server.Settings = ss;
                server.Start();

                var req = new TestPOSTRequest(rs);
                var ok = req.Process();
                if (!ok) throw new ApplicationException("SendPOST() failed. LastError=" + req.LastError);
            }
        }

        public static void SendPOST_AuthFailed()
        {
            var ss = new WebServerSettings()
            {
                Port = 9090,
                UseSSL = false,
                UseBasicAuthorization = true
            };
            var rs = new RequestSettings()
            {
                ServerAddress = "localhost:9090",
                TimeoutMSec = 5000,
                UseSSL = false,
                UseBasicAuthorization = true,
                UserId = "test",
                Password = "1234!"
            };
            using (var server = new TestWebServer())
            {
                server.Settings = ss;
                server.Start();

                var req = new TestPOSTRequest(rs);
                var ok = req.Process();
                if (ok) throw new ApplicationException("SendPOST()_AuthFailed failed");
                if (string.IsNullOrEmpty(req.LastError)) throw new ApplicationException("SendPOST()_AuthFailed failed");
            }
        }
    }
}
