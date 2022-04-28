using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tubumu.Meeting.Server;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MeetingServerServiceCollectionExtensions
    {
        public static IServiceCollection AddMeetingServer(this IServiceCollection services, Action<MeetingServerOptions>? configure = null)
        {
            var meetingServerOptions = new MeetingServerOptions();
            return AddMeetingServer(services, meetingServerOptions, configure);
        }
        public static IServiceCollection AddMeetingServer(this IServiceCollection services, IConfiguration configuration, Action<MeetingServerOptions>? configure = null)
        {
            var meetingServerOptions = new MeetingServerOptions();
            services.AddMediasoup(configuration);
            Configure(services, meetingServerOptions, configuration);
            return AddMeetingServer(services, meetingServerOptions, configure);
        }

        public static IServiceCollection AddMeetingServer(this IServiceCollection services, MeetingServerOptions meetingServerOptions, Action<MeetingServerOptions>? configure = null)
        {
            configure?.Invoke(meetingServerOptions);
            services.AddSingleton(meetingServerOptions);
            services.AddSingleton<Scheduler>();
            services.AddSingleton<BadDisconnectSocketService>();
            return services;
        }

        private static void Configure(this IServiceCollection services, MeetingServerOptions meetingServerOptions, IConfiguration configuration)
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            })
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
                    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.Replace(ServiceDescriptor.Singleton(typeof(IUserIdProvider), typeof(NameUserIdProvider)));

            var meetingServerSettings = configuration.GetSection("MeetingServerSettings").Get<MeetingServerSettings>();
            meetingServerOptions.ServeMode = meetingServerSettings.ServeMode;
        }
    }
}
