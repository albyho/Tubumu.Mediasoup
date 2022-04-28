using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Tubumu.Meeting.Server;
using Tubumu.Meeting.Server.Authorization;

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

            #region Mediasoup configure

            services.AddMeetingServer(Configuration);

            #endregion

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

            #region Mediasoup configure

            app.UseMeetingServer();

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/Health", async context =>
                {
                    await context.Response.WriteAsync("ok");
                });
            });
        }
    }
}
