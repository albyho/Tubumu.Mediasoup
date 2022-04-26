using System;
using Tubumu.Meeting.Server;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeetingServerServiceCollectionExtensions
    {
        public static IServiceCollection AddMeetingServer(this IServiceCollection services, Action<MeetingServerOptions>? configure = null)
        {
            var meetingServerOptions = new MeetingServerOptions();
            configure?.Invoke(meetingServerOptions);
            services.AddSingleton(meetingServerOptions);
            services.AddSingleton<Scheduler>();
            services.AddSingleton<BadDisconnectSocketService>();
            return services;
        }
    }
}
