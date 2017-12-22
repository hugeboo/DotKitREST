using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotKit.RESTclient
{
    /// <summary>
    /// Параметры запроса
    /// </summary>
    public sealed class RequestSettings : ICloneable
    {
        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// Таймаут ожидания ответа от сервера
        /// </summary>
        public int TimeoutMSec { get; set; }

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
        /// Имя (для авторизации)
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Пароль (для авторизации)
        /// </summary>
        public string Password { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
