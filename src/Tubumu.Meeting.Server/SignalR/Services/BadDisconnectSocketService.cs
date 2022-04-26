using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Tubumu.Meeting.Server
{
    public class BadDisconnectSocketService
    {
        private readonly ILogger<BadDisconnectSocketService> _logger;
        private readonly Dictionary<string, HubCallerContext> _cache = new Dictionary<string, HubCallerContext>();
        private readonly object _cacheLock = new object();

        public BadDisconnectSocketService(ILogger<BadDisconnectSocketService> logger)
        {
            _logger = logger ??
               throw new ArgumentNullException(nameof(logger));
        }

        public void DisconnectClient(string connectionId)
        {
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(connectionId, out var context))
                {
                    // 也许连接已关闭，但也再操作一次。
                    context.Abort();
                    _cache.Remove(connectionId);
                }
            }
        }

        public void CacheContext(HubCallerContext context)
        {
            lock (_cacheLock)
            {
                _cache[context.ConnectionId] = context;
            }
        }
    }
}
