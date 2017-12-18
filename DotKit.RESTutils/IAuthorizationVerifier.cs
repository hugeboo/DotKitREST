using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotKit.RESTutils
{
    public interface IAuthorizationVerifier
    {
        bool Verify(string userId, string password);
    }
}
