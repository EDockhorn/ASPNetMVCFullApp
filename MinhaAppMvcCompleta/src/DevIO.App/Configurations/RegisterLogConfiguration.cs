using KissLog.AspNetCore;
using KissLog.CloudListeners.Auth;
using KissLog.CloudListeners.RequestLogsListener;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Text;

namespace DevIO.App.Configurations
{
    public static class RegisterLogConfiguration
    {
        public static IApplicationBuilder UseRegisterLogConfig(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseKissLogMiddleware(options =>
            {
                ConfigureKissLog(options, configuration);
            });

            return app;
        }

        private static void ConfigureKissLog(IOptionsBuilder options, IConfiguration configuration)
        {
            options.Options
                .AppendExceptionDetails((Exception ex) =>
                {
                    var sb = new StringBuilder();

                    if (ex is NullReferenceException nullRefException)
                    {
                        sb.AppendLine("Important: check for null references");
                    }

                    return sb.ToString();
                });

            // KissLog internal logs
            options.InternalLog = (message) =>
            {
                Debug.WriteLine(message);
            };

            // register logs output
            RegisterKissLogListeners(options, configuration);
        }

        private static void RegisterKissLogListeners(IOptionsBuilder options, IConfiguration configuration)
        {
            options.Listeners.Add(new RequestLogsApiListener
                (new Application(configuration["KissLog.OrganizationId"], configuration["KissLog.ApplicationId"]))
            {
                ApiUrl = configuration["KissLog.ApiUrl"]
            });
        }
    }
}
