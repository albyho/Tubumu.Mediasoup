using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tubumu.Libuv;
using Tubumu.Mediasoup;
using Tubumu.Meeting.Server;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMeetingServer(this IApplicationBuilder app)
        {
            // SignalR
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MeetingHub>("/hubs/meetingHub");
            });

            app.UseMediasoup();

            return app;
        }
    }
}
