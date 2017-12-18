using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using DotKit.RESTutils;

namespace DotKit.RESTserver
{
    /// <summary>
    /// Абстрактный обработчик запросов
    /// </summary>
    public abstract class ARequestProcessor
    {
        public WebServer WebServer { get; private set; }

        public ARequestProcessor(WebServer webServer)
        {
            WebServer = webServer;
        }

        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <param name="ctx">Контекст запроса. При успешном выполнении результат записывается в него же.</param>
        /// <returns>True - если смог выполнить запрос (подошел путь, тип и все такое)</returns>
        public abstract bool Execute(HttpListenerContext ctx);

        /// <summary>
        /// Возвращает прослушиваемый путь для настройки HttpListener
        /// </summary>
        public abstract string GetPathForListener();

        /// <summary>
        /// Проверка - подходит ли путь запроса к данному обработчику
        /// </summary>
        protected abstract bool CheckPath(string path);

        /// <summary>
        /// Записать ответ в контекст запроса (при этом ответ автоматом улетает клиенту)
        /// </summary>
        /// <param name="ctx">Контекст запроса</param>
        /// <param name="response">Ответ. Если ноль, то клиенту будет возвращена ошибка InternalServerError</param>
        protected void WriteResponse(HttpListenerContext ctx, object response, Encoding encoding)
        {
            if (response != null)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                var responseStr = DataEncoder.Default.ObjectToStringPOST(response);
                var msg = encoding.GetBytes(responseStr);
                ctx.Response.ContentLength64 = msg.Length;
                using (var s = ctx.Response.OutputStream)
                {
                    s.Write(msg, 0, msg.Length);
                }
            }
            else
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
