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
    /// Абстрактный POST-запрос
    /// </summary>
    /// <typeparam name="Tresponse">Тип объекта, возвращаемого в качестве ответа</typeparam>
    public abstract class APOSTRequest<Tresponse> : ARequest<Tresponse>
    {
        public APOSTRequest(RequestSettings settings)
            : base(settings)
        {
        }

        public override Tresponse Execute(string path, object data)
        {
            var postData = DataEncoder.Default.ObjectToStringPOST(data);
            var byteArray = Encoding.UTF8.GetBytes(postData);
            //var ss = Encoding.UTF8.GetString(byteArray);

            var httpAddr = string.Format(_sAddrFormat, _Settings.ServerAddress, path);
            var request = MakeRequest(httpAddr, "POST");

            request.ContentType = "application/json; charset=UTF-8";
            request.ContentLength = byteArray.Length;

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            var responseStr = GetResponse(request);
            return !string.IsNullOrEmpty(responseStr) ? DataEncoder.Default.ObjectFromStringPOST<Tresponse>(responseStr) : default(Tresponse);
        }
    }
}
