using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace Tubumu.Meeting.Server
{
    public static class SignalRExtensions
    {
        public static HttpContext? GetHttpContext(this HubCallerContext context)
        {
            return context?.Features.Select(x => x.Value as IHttpContextFeature).FirstOrDefault(x => x != null)?.HttpContext;
        }

        public static Dictionary<string, object> ToDictionary(this IQueryCollection httpQuery)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var item in httpQuery)
            {
                dictionary.Add(item.Key, item.Value);
            }
            return dictionary;
        }

        public static void FillToDictionary(this IQueryCollection httpQuery, Dictionary<string, object> dictionary)
        {
            foreach (var item in httpQuery)
            {
                dictionary.Add(item.Key, item.Value);
            }
        }
    }
}
