using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using psg_automated_nunit_common.Configurations;
using psg_automated_nunit_common.Helpers;
using psg_automated_nunit_common.Services;

namespace psg_automated_nunit_common.Extensions
{
    public static class ServiceCollectionExtensions
    { 
        public static void AddStorage(this WebApplicationBuilder builder)
        {
            ConfigurationHelper.AddConfigurations<StorageConfiguration>(builder.Services, "WriteTo");

            builder.Services.AddScoped<StorageService>();
        }            

    }
}
