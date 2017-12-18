using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotKit.RESTutils;

namespace DotKit.RESTserver.Tests
{
    internal sealed class TestAuthorizationVerifier : IAuthorizationVerifier
    {
        public string UserId { get; set; }
        public string Password { get; set; }

        public TestAuthorizationVerifier()
        {
            UserId = "test";
            Password = "1234";
        }

        public bool Verify(string userId, string password)
        {
            return string.Equals(UserId, userId) &&
                string.Equals(Password, password);
        }
    }
}
