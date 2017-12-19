using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

using NLog;
using DotKit.RESTutils;

namespace DotKit.RESTserver
{
    /// <summary>
    /// REST-сервер
    /// </summary>
    public class WebServer : IDisposable
    {
        /// <summary>
        /// Изменилось внутренне состояние сервера
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<Type, ARequestProcessor> _dictProcessors = new Dictionary<Type, ARequestProcessor>();

        private string _csPrefixTemplate;
        private HttpListener _Listener;
        private Thread _ListenerThread;
        private WebServerSettings _Settings;

        /// <summary>
        /// Настройки сервера
        /// </summary>
        public WebServerSettings Settings
        {
            get { return _Settings; }
            set
            {
                if (value != null)
                {
                    _Settings = value;
                    logger.Trace("Settings set");
                }
                else
                {
                    _Settings = new WebServerSettings();
                    logger.Trace("Settings reset to default");
                }
                _csPrefixTemplate = _Settings.UseSSL ? "https://*:{0}{1}/" : "http://*:{0}{1}/";
            }
        }

        /// <summary>
        /// Контролер прав доступа (для авторизации)
        /// </summary>
        public IAuthorizationVerifier AuthorizationVerifier { get; set; }

        /// <summary>
        /// Внутреннее состояние сервера
        /// </summary>
        public WebServerStatus Status { get; private set; }

        public WebServer()
        {
            logger.Info("==== Create new WebServer ====");
            Settings = null;
            _Listener = new HttpListener();
            logger.Trace("WebServer created");
            ChangeStatus(WebServerStatus.Stopped, null);
        }

        public void Dispose()
        {
            Stop();
            logger.Trace("WebServer disposed");
        }

        /// <summary>
        /// Регистрация новго обработчика запросов.
        /// Доступно только при остановленном сервере
        /// </summary>
        public void RegisterRequestProcessor(Type requestProcessorType)
        {
            if (requestProcessorType == null) throw new ArgumentNullException("requestProcessorType");
            if (!requestProcessorType.IsSubclassOf(typeof(ARequestProcessor))) throw new ArgumentException("requestProcessorType must be ARequestProcessor");
            if (Status == WebServerStatus.Working) throw new ApplicationException("WebServer must be stopped");
            var processor = Activator.CreateInstance(requestProcessorType, this) as ARequestProcessor;
            if (processor == null) throw new ApplicationException("Cannot create instance of " + requestProcessorType.FullName);
            _dictProcessors[requestProcessorType] = processor;
        }

        /// <summary>
        /// Запуск сервера (регистрация путей обработчиков и начало прослушивание порта)
        /// </summary>
        public virtual void Start()
        {
            try
            {
                logger.Trace("WebServer starting");

                ChangeStatus(WebServerStatus.Working, "TCP port: " + Settings.Port);
                ChangeStatus(WebServerStatus.Working, "Starting");

                var phs = new HashSet<string>();
                foreach (var rp in _dictProcessors.Values)
                {
                    phs.Add(string.Format(_csPrefixTemplate, Settings.Port, rp.GetPathForListener()));
                }

                if (_Settings.UseBasicAuthorization)
                {
                    _Listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
                }

                _Listener.Prefixes.Clear();
                foreach (var rp in phs)
                {
                    _Listener.Prefixes.Add(rp);
                    logger.Trace("Prefix '{0}' added", rp);
                }

                _Listener.Start();
                _ListenerThread = new Thread(ListenerThreadProc);
                _ListenerThread.Start();
                logger.Info("WebServer started");
                ChangeStatus(WebServerStatus.Working, "Started");
            }
            catch (Exception ex)
            {
                logger.Fatal("Cannot start WebServer", ex);
                ChangeStatus(WebServerStatus.Error, "Cannot start WebServer");
            }
        }

        /// <summary>
        /// Остановка сервера
        /// </summary>
        public void Stop()
        {
            try
            {
                logger.Trace("WebServer stopping");

                var lst = _Listener;
                if (lst != null) lst.Stop();

                var lt = _ListenerThread;
                if (lt != null) lt.Join(3000);

                logger.Info("WebServer stopped");
            }
            catch (Exception ex)
            {
                logger.Error("Error stopping WebServer", ex);
            }
            finally
            {
                ChangeStatus(WebServerStatus.Stopped, "Stopped");
            }
        }

        private void ListenerThreadProc()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var ctx = _Listener.GetContext();
                        ThreadPool.QueueUserWorkItem(ProcessRequest, ctx);
                    }
                    catch (HttpListenerException ex)
                    {
                        logger.Info(ex);
                        break;
                    }
                    catch (InvalidOperationException ex)
                    {
                        logger.Info(ex);
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                logger.Info("ListenerThread aborted");
            }
            catch (Exception ex)
            {
                logger.Fatal("ListenerThread fatal error", ex);
                ChangeStatus(WebServerStatus.Error, "ListenerThread fatal error");
            }
        }

        private void ChangeStatus(WebServerStatus status, string msg)
        {
            Status = status;
            var eh = StatusChanged;
            if (eh != null) eh(this, new StatusChangedEventArgs(status, msg));
        }

        private void ProcessRequest(object listenerContext)
        {
            var ctx = listenerContext as HttpListenerContext;
            try
            {
                if (ctx == null) throw new ArgumentNullException("(listenerContext as HttpListenerContext) == null");

                logger.Debug("ProcessRequest to '{0}' from '{1}'", ctx.Request.RawUrl, ctx.Request.RemoteEndPoint.ToString());

                if (Settings.UseBasicAuthorization)
                {
                    bool ok = false;
                    try
                    {
                        var identity = (HttpListenerBasicIdentity)ctx.User.Identity;
                        if (null != identity)
                        {
                            ok = AuthorizationVerifier.Verify(identity.Name, identity.Password);
                        }

                        //var ps2 = ctx.Request.Headers["Authorization"];
                        //if (!string.IsNullOrEmpty(ps2) && ps2.Length > 10)
                        //{
                        //    ps2 = ps2.Substring(6);
                        //    var ps = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(ps2));
                        //    var splits = ps.Split(':');
                        //    if (splits.Length == 2)
                        //    {
                        //        ok = AuthorizationVerifier.Verify(splits[0], splits[1]);
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Authorization error", ex);
                        ok = false;
                    }

                    if (!ok)
                    {
                        logger.Debug("Authorization failed");
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        ctx.Response.StatusDescription = "Authorization failed";
                        ctx.Response.Close();
                        return;
                    }
                }

                foreach (var processor in _dictProcessors.Values)
                {
                    if (processor.Execute(ctx))
                    {
                        //ctx.Response.Close();
                        return;
                    }
                }

                logger.Debug("RequestProcessor not found");
                ctx.Response.Abort();
            }
            catch (Exception ex)
            {
                string msg;
                if (ctx != null)
                {
                    msg = string.Format("ProcessRequest to '{0}' from '{1}' throw exception",
                        ctx.Request.RawUrl, ctx.Request.RemoteEndPoint.ToString());
                    try
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        ctx.Response.StatusDescription = "Internal exception";
                        ctx.Response.Close();
                    }
                    catch { }
                }
                else
                {
                    msg = "ProcessRequest error (ctx==null)";
                }
                logger.Error(msg, ex);
                ChangeStatus(WebServerStatus.Warning, msg);
            }
        }

        /// <summary>
        /// Состояние сервера
        /// </summary>
        public enum WebServerStatus
        {
            /// <summary>
            /// Оствновлен
            /// </summary>
            Stopped, 

            /// <summary>
            /// Работает
            /// </summary>
            Working, 

            /// <summary>
            /// Работает, но зафиксированы ошибки
            /// </summary>
            Warning, 

            /// <summary>
            /// Остановлен в связи с ошибкой
            /// </summary>
            Error
        }

        /// <summary>
        /// Параметры события об изменении состояния сервера
        /// </summary>
        public sealed class StatusChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Сообщение о состоянии
            /// </summary>
            public readonly string Message;

            /// <summary>
            /// Текущее состояние
            /// </summary>
            public readonly WebServerStatus Status;

            public StatusChangedEventArgs(WebServerStatus status, string msg)
            {
                Status = status;
                Message = msg;
            }
        }
    }
}
