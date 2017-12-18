using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;

using DotKit.RESTutils;

namespace DotKit.RESTclient
{
    /// <summary>
    /// Абстрактный DELETE-запрос
    /// </summary>
    /// <typeparam name="Tresponse">Тип объекта, возвращаемого в качестве ответа</typeparam>
    public abstract class ADELETERequest<Tresponse> : ARequest<Tresponse>
    {
        public ADELETERequest(RequestSettings settings)
            : base(settings)
        {
        }

        public override Tresponse Execute(string path, object data)
        {
            var httpAddr = string.Format(_sAddrFormat, _Settings.ServerAddress, path);
            httpAddr += DataEncoder.Default.ObjectToStringGET(data);
            var request = MakeRequest(httpAddr, "DELETE");

            var responseStr = GetResponse(request);
            return !string.IsNullOrEmpty(responseStr) ? DataEncoder.Default.ObjectFromStringPOST<Tresponse>(responseStr) : default(Tresponse);
        }
    }
}
