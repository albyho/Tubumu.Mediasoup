using System.Collections.Generic;
using System.Text;

namespace System.Collections.Specialized
{
    /// <summary>
    /// NameValueCollectionExtensions
    /// </summary>
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// IsTrue
        /// </summary>
        public static bool IsTrue(this NameValueCollection collection, string key)
        {
            if (collection == null)
            {
                return false;
            }

            var values = collection.GetValues(key);
            if (values.IsNullOrEmpty())
            {
                return false;
            }

            return bool.TryParse(values![0], out var isTrue) && isTrue;
        }

        /// <summary>
        /// IsTrueNullable
        /// </summary>
        public static bool? IsTrueNullable(this NameValueCollection collection, string key)
        {
            if (collection == null)
            {
                return null;
            }

            var values = collection.GetValues(key);
            return values.IsNullOrEmpty() ? null : bool.TryParse(values![0], out bool isTrueValue) && isTrueValue;
        }

        /// <summary>
        /// ToQueryString
        /// </summary>
        public static string ToQueryString(this NameValueCollection queryString)
        {
            if (queryString.Count > 0)
            {
                var qs = new StringBuilder();
                qs.Append('?');
                for (var i = 0; i < queryString.Count; i++)
                {
                    if (i > 0)
                    {
                        qs.Append('&');
                    }

                    qs.AppendFormat("{0}={1}", queryString.Keys[i], queryString[i]);
                }

                return qs.ToString();
            }

            return string.Empty;
        }
    }
}
