using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using DotKit.RESTutils;

namespace DotKit.RESTclient
{
    /// <summary>
    /// Абстрактный запрос
    /// </summary>
    /// <typeparam name="Tresponse">Тип объекта, возвращаемого в качестве ответа</typeparam>
    public abstract class ARequest<Tresponse>
    {
        protected RequestSettings _Settings;
        protected readonly string _sAddrFormat;

        /// <summary>
        /// Ошибка последнего запроса, или ноль, если последний запрос выполнен успешно
        /// </summary>
        public string LastError { get; private set; }

        static ARequest()
        {
            // игнорируем проверку сертификатов в режиме https, используем их только для шифрования трафика
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            //{
            //    return true;
            //};
        }

        public ARequest(RequestSettings settings)
        {
            _Settings = settings;
            _sAddrFormat = settings.UseSSL ? "https://{0}{1}" : "http://{0}{1}";
        }

        /// <summary>
        /// Синхронно выполнить запрос
        /// </summary>
        /// <param name="path">Путь RESTapi внутри сервера</param>
        /// <param name="data">Передаваемые данные</param>
        /// <returns>Ответ сервера, или ноль в случае ошибки</returns>
        public abstract Tresponse Execute(string path, object data);

        protected WebRequest MakeRequest(string httpAddr, string method)
        {
            var request = WebRequest.Create(httpAddr);
            request.Timeout = _Settings.TimeoutMSec;
            request.Method = method;

            if (_Settings.UseBasicAuthorization)
            {
                var ps = _Settings.UserId + ":" + _Settings.Password;
                var ps2 = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ps));
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + ps2);
            }

            return request;
        }

        protected string GetResponse(WebRequest request)
        {
            try
            {
                //if (_Settings.UseBasicAuthorization)
                //{
                //    //(request as HttpWebRequest).
                //    var ps = _Settings.UserId + ":" + _Settings.Password;
                //    var ps2 = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ps));
                //    request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + ps2);
                //}
                using (var response = request.GetResponse())
                {
                    var httpResponse = response as HttpWebResponse;
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        LastError = httpResponse.StatusCode.ToString();
                        if (!string.IsNullOrEmpty(httpResponse.StatusDescription)) LastError += ": " + httpResponse.StatusDescription;
                        return null;
                    }
                    else
                    {
                        using (var dataStream = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStream, Encoding.UTF8))
                            {
                                var responseContent = reader.ReadToEnd();
                                LastError = null;
                                return responseContent;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = "Exception: " + ex.Message;
                return null;
            }
        }
    }
}
