using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Newtonsoft.Json;

namespace DotKit.RESTutils
{
    public static class DataEncoder
    {
        private static ADataEncoder _DefaultEncoder;

        public static ADataEncoder Default
        {
            get { return _DefaultEncoder; }
        }

        static DataEncoder()
        {
            _DefaultEncoder = new JsonDataEncoder();
        }
    }
}
