using System.IO;
using System.Net;

namespace OLinqProvider
{
    internal static class RequestHelper
    {
        internal static string AddParameter(this string url, string param)
        {
            return string.Format(url.Contains("?") ? "{0}&{1}" 
                : "{0}?{1}", url, param);
        }

        internal static string Get(string url)
        {
            var request = WebRequest.Create(url);
            request.ContentType = "application/json";

            string results;
            using (var response = request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        results = reader.ReadToEnd();
                    }
                }
            }
            return results;
        }
    }
}
