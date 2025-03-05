using Microsoft.Extensions.DependencyInjection;

namespace Psg.Standardised.Api.Common.Services
{
    public class Lazier<T> : Lazy<T> where T : class
    {
        public Lazier(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>())
        {
        }
    }
}