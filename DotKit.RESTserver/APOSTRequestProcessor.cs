using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using DotKit.RESTutils;

namespace DotKit.RESTserver
{
    /// <summary>
    /// Абстрактный обработчик POST-запросов.
    /// Для использования в производном классе необходимо реализовать метод Execute
    /// </summary>
    public abstract class APOSTRequestProcessor<Trequest> : ARequestProcessor
    {
        public APOSTRequestProcessor(WebServer webServer)
            : base(webServer)
        {
        }
        
        public override bool Execute(HttpListenerContext ctx)
        {
            if (ctx.Request.HttpMethod == "POST" && CheckPath(ctx.Request.Url.AbsolutePath))
            {
                string requestStr;
                using (var s = ctx.Request.InputStream)
                {
                    using (var reader = new StreamReader(s, Encoding.UTF8, true))
                    {
                        requestStr = reader.ReadToEnd();
                    }
                }
                var request = DataEncoder.Default.ObjectFromStringPOST<Trequest>(requestStr);
                string userName = null;
                var identity = (HttpListenerBasicIdentity)ctx.User.Identity;
                if (null != identity)
                {
                    userName = identity.Name;
                }
                var parameters = DataEncoder.Default.DataFromStringGET(ctx.Request.Url.Query);
                var response = Execute(request, parameters, ctx.Request.Url.AbsolutePath, userName);
                WriteResponse(ctx, response, ctx.Request.ContentEncoding);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <param name="request">Объект-запрос</param>
        /// <param name="parameters">Запрос в виде набора: ключ-значение (как методе GET)</param>
        /// <param name="path">Путь, по которому пришел запрос</param>
        /// <param name="userName">Имя пользователя по результатам авторизации</param>
        /// <returns>Объект-ответ</returns>
        protected abstract object Execute(Trequest request, IDictionary<string, string> parameters, string path, string userName);
    }
}
