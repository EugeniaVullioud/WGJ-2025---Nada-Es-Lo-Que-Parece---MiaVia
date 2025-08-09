using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    // Dictionary to hold pools for each struct type
    private readonly Dictionary<Type, object> pools = new Dictionary<Type, object>();

    // Get or create a pool for a specific type of struct with a creation method
    public EventPool<T> GetPool<T>(Func<T> createInstance) where T : struct
    {
        var type = typeof(T);

        if (!pools.ContainsKey(type))
        {
            // If pool does not exist, create a new one with the creation method
            var pool = new EventPool<T>(createInstance);
            pools[type] = pool;
        }

        return (EventPool<T>)pools[type];
    }

    // Clear all pools
    public void ClearAllPools<T>() where T : struct
    {
        foreach (var pool in pools.Values)
        {
            if (pool is EventPool<T> genericPool)
            {
                genericPool.Clear();
            }
        }
        pools.Clear();
    }
}
