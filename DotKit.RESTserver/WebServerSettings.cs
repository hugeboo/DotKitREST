using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotKit.RESTserver
{
    /// <summary>
    /// Настройки REST-сервера
    /// </summary>
    public class WebServerSettings
    {
        /// <summary>
        /// Прослушиваемый порт
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Использовать протокол HTTPS
        /// </summary>
        public bool UseSSL { get; set; }

        /// <summary>
        /// Использовать авторизацию: HTTP basic authentication
        /// http://en.wikipedia.org/wiki/Basic_access_authentication
        /// </summary>
        public bool UseBasicAuthorization { get; set; }

        /// <summary>
        /// Интервал между запросами данных в long polling запросах
        /// </summary>
        public int LongPollingIntervalMSec { get; set; }

        /// <summary>
        /// Максимальное кол-во запросов данных в long polling запросе
        /// </summary>
        public int LongPollingMaxCount { get; set; }

        public WebServerSettings()
        {
            Port = 8086;
            LongPollingIntervalMSec = 1000;
            LongPollingMaxCount = 10;
        }
    }
}
