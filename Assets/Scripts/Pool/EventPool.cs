using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPool<T> where T : struct
{
    private Queue<T> pool = new Queue<T>();

    private readonly Func<T> createInstance;

    // Constructor that accepts a creation method
    public EventPool(Func<T> createInstance)
    {
        this.createInstance = createInstance ?? throw new ArgumentNullException(nameof(createInstance));
    }

    // Get a pooled event or create a new one using the provided method if the pool is empty
    public T Get()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        // Use the provided creation method to create a new struct
        return createInstance();
    }

    // Return an event struct to the pool
    public void Return(T item)
    {
        pool.Enqueue(item);
    }

    // Clear the pool
    public void Clear()
    {
        pool.Clear();
    }
}