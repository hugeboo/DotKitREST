using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

using DotKit.RESTutils;

namespace DotKit.RESTserver
{
    /// <summary>
    /// Абстрактный обработчик GET-запросов с функцией long polling.
    /// Для использования в производном классе необходимо реализовать метод Execute
    /// </summary>
    public abstract class AGETRequestProcessorWithLongPolling : ARequestProcessor
    {
        public AGETRequestProcessorWithLongPolling(WebServer webServer)
            : base(webServer)
        {
        }

        public override bool Execute(HttpListenerContext ctx)
        {
            if (ctx.Request.HttpMethod == "GET" && CheckPath(ctx.Request.Url.AbsolutePath))
            {
                var parameters = DataEncoder.Default.DataFromStringGET(ctx.Request.Url.Query);
                string userName = null;
                var identity = (HttpListenerBasicIdentity)ctx.User.Identity;
                if (null != identity)
                {
                    userName = identity.Name;
                }

                object response = null;
                for (int i = 0; i < WebServer.Settings.LongPollingMaxCount; i++)
                {
                    if (TryExecute(parameters, ctx.Request.Url.AbsolutePath, userName, out response)) break;
                    Thread.Sleep(WebServer.Settings.LongPollingIntervalMSec);
                }
                
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
        /// <param name="response">Объект-ответ</param>
        /// <returns>Если false, то считаем, что ответ еще не готов, и через PollingIntervalMSec делаем новый запрос</returns>
        protected abstract bool TryExecute(IDictionary<string, string> parameters, string path, string userName, out object response);
    }
}
