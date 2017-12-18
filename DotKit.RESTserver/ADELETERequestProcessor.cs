using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using DotKit.RESTutils;

namespace DotKit.RESTserver
{
    /// <summary>
    /// Абстрактный обработчик DELETE-запросов.
    /// Для использования в производном классе необходимо реализовать метод Execute
    /// </summary>
    public abstract class ADELETERequestProcessor : ARequestProcessor
    {
        public ADELETERequestProcessor(WebServer webServer)
            : base(webServer)
        {
        }

        public override bool Execute(HttpListenerContext ctx)
        {
            if (ctx.Request.HttpMethod == "DELETE" && CheckPath(ctx.Request.Url.AbsolutePath))
            {
                var parameters = DataEncoder.Default.DataFromStringGET(ctx.Request.Url.Query);
                string userName = null;
                var identity = (HttpListenerBasicIdentity)ctx.User.Identity;
                if (null != identity)
                {
                    userName = identity.Name;
                }
                var response = Execute(parameters, ctx.Request.Url.AbsolutePath, userName);
                WriteResponse(ctx, response, Encoding.UTF8);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <param name="parameters">Запрос в виде набора: ключ-значение</param>
        /// <param name="path">Путь, по которому пришел запрос</param>
        /// <param name="userName">Имя пользователя по результатам авторизации</param>
        /// <returns>Объект-ответ</returns>
        protected abstract object Execute(IDictionary<string, string> parameters, string path, string userName);
    }
}
