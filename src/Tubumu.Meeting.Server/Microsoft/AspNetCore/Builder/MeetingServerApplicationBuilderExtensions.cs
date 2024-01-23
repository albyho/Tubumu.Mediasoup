using Tubumu.Meeting.Server;

namespace Microsoft.AspNetCore.Builder
{
    public static class MeetingServerApplicationBuilderExtensions
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
