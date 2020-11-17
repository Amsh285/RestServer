using System.Collections;
using System.Collections.Generic;

namespace RestServer.WebServer.Infrastructure
{
    public class Multimap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
    {
        public List<TValue> this[TKey key]
        {
            get
            {
                return internalMap[key];
            }
            set
            {
                Assert.NotNull(value, nameof(value));

                foreach (TValue item in value)
                    Add(key, item);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return internalMap.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (internalMap.ContainsKey(key))
                internalMap[key].Add(value);
            else
                internalMap.Add(key, new List<TValue>() { value });
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> range)
        {
            foreach (KeyValuePair<TKey, TValue> item in range)
                Add(item.Key, item.Value);
        }

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        {
            return internalMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalMap.GetEnumerator();
        }

        private readonly Dictionary<TKey, List<TValue>> internalMap = new Dictionary<TKey, List<TValue>>();
    }
}
