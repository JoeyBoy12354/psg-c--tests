using Microsoft.Extensions.Caching.Memory;
using Psg.Standardised.Api.Common.Configurations;
using System.Collections.Concurrent;

namespace Psg.Standardised.Api.Common.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();    
        private readonly MemoryCacheEntryOptions _options;

        public CacheService(IMemoryCache memoryCache,                            
                            CacheConfiguration cacheConfiguration)
        {
            _memoryCache = memoryCache;           
            _options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(cacheConfiguration.AbsoluteExpirationHours),
                Size = 1
            };           
        }

        public async Task<(T item, bool itemCreated)> GetOrCreate<T>(object key, Func<Task<T>> createItem)
        {
            T cacheEntry;
            bool itemCreated = false;

            if (!_memoryCache.TryGetValue(key, out cacheEntry))
            {
                var mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

                await mylock.WaitAsync();
                try
                {
                    if (!_memoryCache.TryGetValue(key, out cacheEntry))
                    {
                        cacheEntry = await createItem();
                       
                        _memoryCache.Set(key, cacheEntry, _options);
                        itemCreated = true;
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }

            return (cacheEntry, itemCreated);
        }      

        public void Add<TEntry>(object key, TEntry cacheEntry)
        {
            _memoryCache.Set(key, cacheEntry, _options);            
        }

        public void Add<TEntry>(object key, TEntry cacheEntry, MemoryCacheEntryOptions options)
        {
            _memoryCache.Set(key, cacheEntry, options);
        }

        public T Get<T>(object key)
        {
            if (_memoryCache.TryGetValue(key, out T entry))
            {
                return entry;
            }

            return default;
        }
    }
}
