using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Tubumu.Mediasoup;
using Tubumu.Meeting.Server;
using Tubumu.Meeting.Server.Authorization;
using Tubumu.Utils.Extensions;
using Tubumu.Utils.Extensions.Ip;

namespace Tubumu.Meeting.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            // Cache
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost";
                options.InstanceName = "Meeting:";
            });
            services.AddMemoryCache();

            // Cors
            var corsSettings = Configuration.GetSection("CorsSettings").Get<CorsSettings>();
            services.AddCors(options => options.AddPolicy("DefaultPolicy",
                builder => builder.WithOrigins(corsSettings.Origins).AllowAnyMethod().AllowAnyHeader().AllowCredentials())
            );

            // Authentication
            services.AddSingleton<ITokenService, TokenService>();
            var tokenValidationSettings = Configuration.GetSection("TokenValidationSettings").Get<TokenValidationSettings>();
            services.AddSingleton(tokenValidationSettings);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = tokenValidationSettings.ValidIssuer,
                        ValidateIssuer = true,

                        ValidAudience = tokenValidationSettings.ValidAudience,
                        ValidateAudience = true,

                        IssuerSigningKey = SignatureHelper.GenerateSigningKey(tokenValidationSettings.IssuerSigningKey),
                        ValidateIssuerSigningKey = true,

                        ValidateLifetime = tokenValidationSettings.ValidateLifetime,
                        ClockSkew = TimeSpan.FromSeconds(tokenValidationSettings.ClockSkewSeconds),
                    };

                    // We have to hook the OnMessageReceived event in order to
                    // allow the JWT authentication handler to read the access
                    // token from the query string when a WebSocket or
                    // Server-Sent Events request comes in.
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            //_logger.LogError($"Authentication Failed(OnAuthenticationFailed): {context.Request.Path} Error: {context.Exception}");
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        },
                        OnChallenge = async context =>
                        {
                            //_logger.LogError($"Authentication Challenge(OnChallenge): {context.Request.Path}");
                            var body = Encoding.UTF8.GetBytes("{\"code\": 400, \"message\": \"Authentication Challenge\"}");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.Body.WriteAsync(body, 0, body.Length);
                            context.HandleResponse();
                        }
                    };
                });

            // SignalR
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

            // Mediasoup
            var mediasoupStartupSettings = Configuration.GetSection("MediasoupStartupSettings").Get<MediasoupStartupSettings>();
            var mediasoupSettings = Configuration.GetSection("MediasoupSettings").Get<MediasoupSettings>();
            var workerSettings = mediasoupSettings.WorkerSettings;
            var routerSettings = mediasoupSettings.RouterSettings;
            var webRtcTransportSettings = mediasoupSettings.WebRtcTransportSettings;
            var plainTransportSettings = mediasoupSettings.PlainTransportSettings;
            var meetingServerSettings = Configuration.GetSection("MeetingServerSettings").Get<MeetingServerSettings>();
            services.AddMediasoup(options =>
            {
                // MediasoupStartupSettings
                if (mediasoupStartupSettings != null)
                {
                    options.MediasoupStartupSettings.MediasoupVersion = mediasoupStartupSettings.MediasoupVersion;
                    options.MediasoupStartupSettings.WorkerInProcess = mediasoupStartupSettings.WorkerInProcess;
                    options.MediasoupStartupSettings.WorkerPath = mediasoupStartupSettings.WorkerPath;
                    options.MediasoupStartupSettings.NumberOfWorkers = !mediasoupStartupSettings.NumberOfWorkers.HasValue || mediasoupStartupSettings.NumberOfWorkers <= 0 ? Environment.ProcessorCount : mediasoupStartupSettings.NumberOfWorkers;
                }

                // WorkerSettings
                if (workerSettings != null)
                {
                    options.MediasoupSettings.WorkerSettings.LogLevel = workerSettings.LogLevel;
                    options.MediasoupSettings.WorkerSettings.LogTags = workerSettings.LogTags;
                    options.MediasoupSettings.WorkerSettings.RtcMinPort = workerSettings.RtcMinPort;
                    options.MediasoupSettings.WorkerSettings.RtcMaxPort = workerSettings.RtcMaxPort;
                    options.MediasoupSettings.WorkerSettings.DtlsCertificateFile = workerSettings.DtlsCertificateFile;
                    options.MediasoupSettings.WorkerSettings.DtlsPrivateKeyFile = workerSettings.DtlsPrivateKeyFile;
                }

                // RouteSettings
                if (routerSettings != null && !routerSettings.RtpCodecCapabilities.IsNullOrEmpty())
                {
                    options.MediasoupSettings.RouterSettings = routerSettings;

                    // Fix RtpCodecCapabilities[x].Parameters 。从配置文件反序列化时将数字转换成了字符串，这里进行修正。
                    foreach (var codec in routerSettings.RtpCodecCapabilities.Where(m => m.Parameters != null))
                    {
                        foreach (var key in codec.Parameters.Keys.ToArray())
                        {
                            var value = codec.Parameters[key];
                            if (value != null && int.TryParse(value.ToString(), out var intValue))
                            {
                                codec.Parameters[key] = intValue;
                            }
                        }
                    }
                }

                // WebRtcTransportSettings
                if (webRtcTransportSettings != null)
                {
                    options.MediasoupSettings.WebRtcTransportSettings.ListenIps = webRtcTransportSettings.ListenIps;
                    options.MediasoupSettings.WebRtcTransportSettings.InitialAvailableOutgoingBitrate = webRtcTransportSettings.InitialAvailableOutgoingBitrate;
                    options.MediasoupSettings.WebRtcTransportSettings.MinimumAvailableOutgoingBitrate = webRtcTransportSettings.MinimumAvailableOutgoingBitrate;
                    options.MediasoupSettings.WebRtcTransportSettings.MaxSctpMessageSize = webRtcTransportSettings.MaxSctpMessageSize;

                    // 如果没有设置 ListenIps 则获取本机所有的 IPv4 地址进行设置。
                    var listenIps = options.MediasoupSettings.WebRtcTransportSettings.ListenIps;
                    if (listenIps.IsNullOrEmpty())
                    {
                        var localIPv4IPAddresses = IPAddressExtensions.GetLocalIPAddresses(AddressFamily.InterNetwork).Where(m => m != IPAddress.Loopback);
                        if (EnumerableExtensions.IsNullOrEmpty(localIPv4IPAddresses))
                        {
                            throw new ArgumentException("无法获取本机 IPv4 配置 WebRtcTransport。");
                        }

                        listenIps = (from ip in localIPv4IPAddresses
                                    let ipString = ip.ToString()
                                    select new TransportListenIp
                                    {
                                        Ip = ipString,
                                        AnnouncedIp = ipString
                                    }).ToArray();
                        options.MediasoupSettings.WebRtcTransportSettings.ListenIps = listenIps;
                    }
                    else
                    {
                        var localIPv4IPAddress = IPAddressExtensions.GetLocalIPv4IPAddress();
                        if (localIPv4IPAddress == null)
                        {
                            throw new ArgumentException("无法获取本机 IPv4 配置 WebRtcTransport。");
                        }

                        foreach (var listenIp in listenIps)
                        {
                            if (listenIp.AnnouncedIp.IsNullOrWhiteSpace())
                            {
                            // 如果没有设置 AnnouncedIp：
                            // 如果 Ip 属性的值不是 Any 则赋值为 Ip 属性的值，否则取本机的任意一个 IPv4 地址进行设置。(注意：可能获取的并不是正确的 IP)
                            listenIp.AnnouncedIp = listenIp.Ip == IPAddress.Any.ToString() ? localIPv4IPAddress.ToString() : listenIp.Ip;
                            }
                        }
                    }
                }

                // PlainTransportSettings
                if (plainTransportSettings != null)
                {
                    options.MediasoupSettings.PlainTransportSettings.ListenIp = plainTransportSettings.ListenIp;
                    options.MediasoupSettings.PlainTransportSettings.MaxSctpMessageSize = plainTransportSettings.MaxSctpMessageSize;

                    var localIPv4IPAddress = IPAddressExtensions.GetLocalIPv4IPAddress();
                    if (localIPv4IPAddress == null)
                    {
                        throw new ArgumentException("无法获取本机 IPv4 配置 PlainTransport。");
                    }

                    var listenIp = options.MediasoupSettings.PlainTransportSettings.ListenIp;
                    if (listenIp == null)
                    {
                        listenIp = new TransportListenIp
                        {
                            Ip = localIPv4IPAddress.ToString(),
                            AnnouncedIp = localIPv4IPAddress.ToString(),
                        };
                        options.MediasoupSettings.PlainTransportSettings.ListenIp = listenIp;
                    }
                    else if (listenIp.AnnouncedIp.IsNullOrWhiteSpace())
                    {
                    // 如果没有设置 AnnouncedIp：
                    // 如果 Ip 属性的值不是 Any 则赋值为 Ip 属性的值，否则取本机的任意一个 IPv4 地址进行设置。(注意：可能获取的并不是正确的 IP)
                    listenIp.AnnouncedIp = listenIp.Ip == IPAddress.Any.ToString() ? localIPv4IPAddress.ToString() : listenIp.Ip;
                    }
                }
            });

            // Meeting server
            services.AddMeetingServer(options =>
            {
                options.ServeMode = meetingServerSettings.ServeMode;
            });

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Meeting API",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("DefaultPolicy");

            // For Testing.
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Tubumu Meeting");
            //    });
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

            // SignalR
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MeetingHub>("/hubs/meetingHub");
            });
            app.UseSigSpec(options => { options.Hubs = new[] { typeof(MeetingHub) }; });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/Health", async context =>
                {
                    await context.Response.WriteAsync("ok");
                });
            });

            // Mediasoup
            app.UseMediasoup();
        }
    }
}
