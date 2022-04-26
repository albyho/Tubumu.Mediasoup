using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Tubumu.Meeting.Server
{
    /// <summary>
    /// NameUserIdProvider
    /// </summary>
    public class NameUserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// GetUserId
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string? GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.FindFirst("id")?.Value ??
                connection.User?.FindFirst("name")?.Value ??
                connection.User?.FindFirst(ClaimTypes.Name)?.Value;
            return userId;
        }
    }
}
