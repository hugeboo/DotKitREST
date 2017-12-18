using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DotKit.RESTutils
{
    /// <summary>
    /// Преобразователь данных запросов и ответов (GET, POST) для использовании в RESTapi.
    /// </summary>
    public abstract class ADataEncoder
    {
        /// <summary>
        /// Преобразование объекта для передачи в теле POST-запроса/ответа
        /// </summary>
        public abstract string ObjectToStringPOST(object obj);

        /// <summary>
        /// Получение объекта из тела POST-запроса/ответа
        /// </summary>
        public abstract T ObjectFromStringPOST<T>(string s);

        /// <summary>
        /// Получение набора ключ-значение из строки GET-запроса
        /// </summary>
        public virtual IDictionary<string, string> DataFromStringGET(string s)
        {
            var ds = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(s))
            {
                var splits = s.Split(new[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var sp in splits)
                {
                    var splits2 = sp.Split('=');
                    if (splits2.Length == 2)
                    {
                        var decodedValue = Uri.UnescapeDataString(splits2[1]);//!!!!
                        decodedValue = decodedValue.Replace('+', ' ');
                        ds[splits2[0]] = decodedValue;
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// Получение из свойств объекта набора ключ-значение и преобразование его в строку GET-запроса
        /// </summary>
        public virtual string ObjectToStringGET(object obj)
        {
            if (obj != null)
            {
                var dict = new Dictionary<string, string>();
                var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var pi in props)
                {
                    var val = pi.GetValue(obj, null);
                    if (val != null)
                    {
                        if (val is DateTime)
                        {
                            dict[pi.Name] = DateToString((DateTime)val);
                        }
                        else if (val is DateTime?)
                        {
                            dict[pi.Name] = DateToString(((DateTime?)val).Value);
                        }
                        else
                        {
                            dict[pi.Name] = val.ToString();
                        }
                    }
                }
                return DataToStringGET(dict);
            }
            return null;
        }
        
        /// <summary>
        /// Преобразование набора ключ-значение в строку GET-запроса
        /// </summary>
        public virtual string DataToStringGET(IDictionary<string, string> data)
        {
            if (data.Count > 0)
            {
                var s = "?";
                if (data.Count > 0)
                {
                    int i = 0;
                    foreach (var kvp in data)
                    {
                        var encodedValue = Uri.EscapeDataString(kvp.Value);//!!!!
                        s += kvp.Key + "=" + encodedValue;
                        if (i < data.Count - 1) s += "&";
                        i += 1;
                    }
                }
                return s;
            }
            return null;
        }

        private readonly string _sFullTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fff";
        private readonly string _sShortTimeFormat = "yyyy-MM-dd";

        /// <summary>
        /// Преобразование даты в строку
        /// </summary>
        public virtual string DateToString(DateTime time)
        {
            if (time.TimeOfDay.TotalMilliseconds > 0.0)
            {
                return time.ToString(_sFullTimeFormat);
            }
            else
            {
                return time.ToString(_sShortTimeFormat);
            }
        }

        /// <summary>
        /// Преобразование строки в дату
        /// </summary>
        public virtual DateTime DateFromString(string s)
        {
            DateTime dt;
            if (s.Length > 10)
            {
                DateTime.TryParseExact(s, _sFullTimeFormat,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                    out dt);
                return dt;
            }
            else
            {
                DateTime.TryParseExact(s, _sShortTimeFormat,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                    out dt);
                return dt.Date;
            }
        }
    }
}
