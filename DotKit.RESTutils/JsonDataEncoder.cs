using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace DotKit.RESTutils
{
    public sealed class JsonDataEncoder : ADataEncoder
    {
        public JsonSerializerSettings _JsonSettings;

        public JsonDataEncoder()
        {
            _JsonSettings = new JsonSerializerSettings();
            _JsonSettings.DateFormatString = "yyyy-MM-dd'T'HH:mm:ss.fff";
        }
        
        public override string ObjectToStringPOST(object obj)
        {
            return JsonConvert.SerializeObject(obj, _JsonSettings);
        }

        public override T ObjectFromStringPOST<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s, _JsonSettings);
        }
    }
}
