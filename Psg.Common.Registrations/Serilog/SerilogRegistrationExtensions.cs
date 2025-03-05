using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Psg.Common.Registrations.Extensions;
using Psg.Common.Registrations.Logging;
using Psg.Common.Registrations.Serilog.Models;
using Serilog;
using Serilog.Context;
using System.Net;

namespace Psg.Common.Registrations.Serilog
{
    /// <summary>
    /// Contains extension methods for PSG registrations
    /// </summary>
    public static class SerilogRegistrationExtensions
    {
        /// <summary>
        /// Add Serilog to the application with default PSG sinks and contexts.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="ApplicationName"></param>
        /// <returns></returns>
        public static SerilogConfigStatus AddSerilogPsg(this WebApplicationBuilder builder,
                                                          string ApplicationName)
        {

            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);

                // setup Serilog static LOG, for when dependency injection isn't available
                Log.Logger = new LoggerConfiguration()
                       .ReadFrom.Configuration(hostingContext.Configuration)
                       .CreateLogger();
            });

            GlobalLogContext.PushProperty("Application", ApplicationName); // the way Serilog wants the name. Used when using Tracing
            GlobalLogContext.PushProperty("ApiName", ApplicationName);
            GlobalLogContext.PushProperty("Environment", builder.Environment.EnvironmentName);
            GlobalLogContext.PushProperty("HostIp", GetHostIpAddress());

            return builder.GetSerilogConfigStatus(ApplicationName);
        }

        /// <summary>
        /// Setup Serilog in projects that do not have dependency injection
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ApplicationName"></param>
        /// <param name="environment"></param>
        public static void AddSerilogPsg(this IConfigurationRoot config,
                                         string ApplicationName,
                                         string? environment = default)
        {
            if(string.IsNullOrWhiteSpace(environment))
            {
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            }           

            // setup Serilog static LOG, for when dependency injection isn't available
            Log.Logger = new LoggerConfiguration()
                   .ReadFrom.Configuration(config)
                   .CreateLogger();

            GlobalLogContext.PushProperty("Application", ApplicationName); // the way Serilog wants the name. Used when using Tracing
            GlobalLogContext.PushProperty("ApiName", ApplicationName);
            GlobalLogContext.PushProperty("Environment", environment);
            GlobalLogContext.PushProperty("HostIp", GetHostIpAddress());
           
        }

        /// <summary>
        /// Add Serilog Tracing to call, which can be viewed in Seq.
        /// </summary>
        /// <param name="app"></param>
        public static void UseSerilogTracingPsg(this WebApplication app)
        {            
            app.UseMiddleware<SerilogPsgTraceMiddleware>();
        }

        /// <summary>
        /// Throw an error if Serilog config is not found in appsettings, or if it does not contain at least 1 'WriteTo' section.
        /// </summary>
        /// <param name="status"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateOnStart(this SerilogConfigStatus status)
        {
            if (!status.ConfigValid)
            {
                if (!string.IsNullOrWhiteSpace(status.Message))
                {
                    throw new ArgumentException(status.Message);
                }
                else
                {
                    throw new ArgumentException("There is an issue with the Serilog config!");
                }
            }
        }


        private static SerilogConfigStatus GetSerilogConfigStatus(this WebApplicationBuilder builder,
                                                                  string ApplicationName)
        {
            var logger = builder.GetStartupLogger();

            SerilogValidationModel model = new();

            builder.Configuration.GetSection("Serilog").Bind(model);

            if (model.WriteTo == null || model.WriteTo.Count == 0)
            {
                return SerilogConfigStatus.Bad("No 'WriteTo' Section found in Serilog Config! Please check Serilog config in appsettings!");
            }

            // Check Logs path
            var path = GetFilePath(model);

            if(!string.IsNullOrWhiteSpace(path))
            {                
                logger.LogInformation("Serilog logs path: {Path}.", path);

                var name = ApplicationName.Replace("Api", "").Trim();
                var newPath = $"D:\\Logs\\Api\\{name}\\log_.txt";

                if (path == "bin\\log_.txt")
                {
                    logger.LogError("Serilog Registration Error! Logs are stored in {Path} and will be deleted with the next deployment!", path);
                    logger.LogError("Please store logs in {Path}", newPath);
                }

                if(path.Contains("C:"))
                {
                    if(!builder.Environment.EnvironmentName.ToLower().Contains("local"))
                    {
                        logger.LogError("Serilog Registration Error! Logs are stored in {Path}", path);
                        logger.LogError("Please try to log to the {Drive} drive", "D:");
                    }                  
                }

                if(path.Contains("ApiName"))
                {
                    logger.LogError("Serilog Registration Error! Logs are stored in {Path}", path);
                    logger.LogError("Please remove {ApiName} and put the name of the Api", "ApiName");
                }
            }

            // check SEQ path

            var seqUrl = GetSeqUrl(model);

            if(!string.IsNullOrEmpty(seqUrl))
            {
                logger.LogInformation("Serilog SEQ Url: {Url}.", seqUrl);

                if (builder.Environment.IsNotLocal())
                {
                    if(seqUrl.ToLower().Contains("local"))
                    {
                        logger.LogError("Environment is {Env}, but SEQ is logging to {Url}.", builder.Environment.EnvironmentName, seqUrl);
                    }
                }
            }
            else
            {
                logger.LogInformation("Not writing to SEQ.");
            }

            logger.LogInformation("Serilog registered.");

            return SerilogConfigStatus.Good;
        }   

        private static string GetFilePath(SerilogValidationModel model)
        {
            var writeTos = model.WriteTo.Where(x => x.Args != null && x.Args.configure != null).ToList();
            var configs = writeTos.Where(x => x.Args?.configure?.Count > 0).ToList();

            return GetFilePath(configs);
        }

        private static string GetFilePath(Args args)
        {
            return GetFilePath(args.configure);
        }

        private static string GetFilePath(List<Configure> configs)
        {
            foreach(var config in configs)
            {
                if (config.Name != "File")
                {
                    return GetFilePath(config.Args);
                }

                if (!string.IsNullOrWhiteSpace(config.Args.path))
                    return config.Args.path;
            }

            return "";
        }


        private static string GetFilePath(List<WriteTo> writeTos)
        {
            foreach(var writeTo in writeTos) 
            { 
                if(writeTo.Name != "File")
                {
                    return GetFilePath(writeTo.Args);
                }

                if (!string.IsNullOrWhiteSpace(writeTo.Args.path))
                    return writeTo.Args.path;
            }            

            return "";
        }

        private static string GetSeqUrl(SerilogValidationModel model)
        {
            var seq = model.WriteTo.Find(x => x.Name == "Seq");

            if (seq == null || seq.Args == null)
                return "";

            return seq.Args?.serverUrl ?? "";
        }


        /// <summary>
        /// Extension method used to add the middleware to the HTTP request pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSerilogPsgMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SerilogPsgMiddleware>();
        }

        /// <summary>
        /// Adds UseSerilogRequestLogging() and UseSerilogPsgMiddleware()
        /// </summary>      
        public static void UseSerilogPsg(this WebApplication app, 
                                         bool RequestLoggingOnly = false,
                                         bool PsgMiddlewareOnly = false)
        {
            if(RequestLoggingOnly)
            {
                app.UseSerilogRequestLogging();
                return;
            }

            if (PsgMiddlewareOnly)
            {
                app.UseSerilogPsgMiddleware();
                return;
            }

            app.UseSerilogRequestLogging();
            app.UseSerilogPsgMiddleware();
        }

        private static string? GetHostIpAddress()
        {
            // Get the host name
            string hostName = Dns.GetHostName();

            // Get the IP addresses associated with the host name
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            // Find the first IPv4 address (assuming you want IPv4)
            foreach (IPAddress addr in addresses)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return addr.ToString();
                }
            }

            // If no IPv4 address found, return null or handle accordingly
            return null;
        }

    }
}
