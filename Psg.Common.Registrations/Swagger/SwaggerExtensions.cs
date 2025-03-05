using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Psg.Common.Registrations.Logging;
using System.Reflection;

namespace Psg.Common.Registrations.Swagger
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Register Swagger with the 'Auth' button. Remember to Add the
        /// <br/> ['GenerateDocumentationFile' = True] element
	    /// <br/> in the project file under 'PropertyGroup'
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="ApplicationName"></param>
        public static void AddSwaggerPsg(this WebApplicationBuilder builder,
                                                         string ApplicationName)
        {
            var logger = builder.GetStartupLogger();
          
            var xmlPath = GetXmlFile(builder);       

            if(!File.Exists(xmlPath)) 
            {
                logger.LogInformation("XmlPath NOT FOUND! Searching for other xml files...");
            }

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = ApplicationName, Version = "v1" });
               
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {{
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }});

                c.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
                c.MapType<decimal?>(() => new OpenApiSchema { Type = "number", Format = "decimal", Nullable = true });
            });

            logger.LogInformation("Swagger registered.");

        }


        private static string GetXmlFile(WebApplicationBuilder builder)
        {
            var logger = builder.GetStartupLogger();

            // first check Psg.Standardised.Api.xml
            var xmlFile = "Psg.Standardised.Api.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            logger.LogInformation("Swagger XmlFile: {XmlPath}", Path.GetFileName(xmlPath));
            logger.LogInformation("Swagger XmlPath: {XmlPath}", xmlPath);

            if (File.Exists(xmlPath))
            {
                return xmlPath;
            }

            // now check the executing assembly....

            var executingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            xmlFile = $"{executingAssemblyName}.xml";
            xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            logger.LogInformation("Swagger Executing assembly name: {ExecutingAssemblyName}", executingAssemblyName);
            logger.LogInformation("Swagger XmlFile: {XmlPath}", Path.GetFileName(xmlPath));
            logger.LogInformation("Swagger XmlPath: {XmlPath}", xmlPath);

            if (File.Exists(xmlPath))
            {
                return xmlPath;
            }

            logger.LogInformation("XmlPath NOT FOUND! Searching for other xml files...");

            // now check for any xml files and use the first one ...
            var files = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml");

            if(!files.Any())
            {
                logger.LogError("No XML files found for Swagger!");
                return "NOVALIDXMLFILESFOUND!";
            }

            logger.LogInformation("[{Count}] Xml files found", files.Count());

            foreach ( var file in files) 
            {
                logger.LogInformation("Filename: {Filename}", Path.GetFileName(file));
            }

            var f = files.First();

            logger.LogInformation("Selecting file: {Filename}", Path.GetFileName(f));

            return f;
        }


    }
}
