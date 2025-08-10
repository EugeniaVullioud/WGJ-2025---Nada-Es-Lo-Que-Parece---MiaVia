using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPool<T> where T : struct
{
    private readonly Queue<T> _objects = new Queue<T>();

    public EventPool(int initialCount = 20)
    {
        for (int i = 0; i < initialCount; i++)
            _objects.Enqueue(default);
    }

    public T Get()
    {
        return _objects.Count > 0 ? _objects.Dequeue() : default;
    }

    public void Return(T obj)
    {
        _objects.Enqueue(obj);
    }

    public void Clear()
    {
        _objects.Clear();
    }
}

    /*GenericPool<T> where T : struct
{  
    public EventPool(Func<T> createFunc, int initialCount = 0)
        : base(createFunc, initialCount)
    {
    }
}*/