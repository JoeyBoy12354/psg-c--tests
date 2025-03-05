namespace Psg.Standardised.Api.Common.Services
{
    /// <summary>
    /// Note, this is not thread safe.
    /// Use this class to quickly cache data locally in a method.
    /// Useful in foreach loops where db calls and myPractice calls are made
    /// </summary>
    /// <typeparam name="TKey">The Key/Id type, usually int or sometimes string</typeparam>
    /// <typeparam name="TValue">The type of the data that is returned</typeparam>
    public class LocalCache<TKey, TValue>
    {
        public int Count => _cache?.Count ?? 0;
        public bool Empty => _cache?.Count == 0;


        private readonly Dictionary<TKey, TValue> _cache;

        public LocalCache()
        {
            _cache = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Retreives the data per key, or fetches and stores it, if it is not in the cache
        /// </summary>
        /// <param name="key">key of the data</param>
        /// <param name="asyncDataRetrievalDelegate">(arrow function) async method that will be used retrieve the data. The Db call or myPractice call...</param>
        /// <returns></returns>
        public async Task<TValue> GetDataAsync(TKey key, Func<Task<TValue>> asyncDataRetrievalDelegate)
        {
            // allows inserting of nulls, as this is to cache values from db calls in for loops,
            // so that it does not make a db call again as it already knows the answer
            // including if the answer is 'null'

            if (!_cache.ContainsKey(key))
            {
                TValue data = await asyncDataRetrievalDelegate();
                _cache[key] = data;
            }

            return _cache[key];
        }      

        /// <summary>
        /// Retreives the data per key, or fetches and stores it, if it is not in the cache
        /// </summary>
        /// <param name="key">key of the data</param>
        /// <param name="dataRetrievalDelegate">(arrow function) method that will be used retrieve the data. The Db call or myPractice call...</param>
        /// <returns></returns>
        public TValue GetData(TKey key, Func<TValue> dataRetrievalDelegate)
        {
            if (!_cache.ContainsKey(key))
            {
                TValue data = dataRetrievalDelegate();
                _cache[key] = data;
            }

            return _cache[key];
        }

        public TValue GetData(TKey key)
        {
            if (_cache.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return default(TValue); // Returns null for reference types, 0 for value types
        }

        /// <summary>
        /// Manually add data to the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void AddOrUpdateData(TKey key, TValue data)
        {
            _cache[key] = data;
        }       

        public List<TValue> ToList()
        {
            return _cache.Values.ToList();
        }

        public bool ContainsKey(TKey key)
        {
            return _cache.ContainsKey(key);
        }
    }
}
