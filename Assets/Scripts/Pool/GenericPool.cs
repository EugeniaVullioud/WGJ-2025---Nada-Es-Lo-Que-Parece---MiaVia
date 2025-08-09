using System;
using System.Collections.Generic;

public abstract class GenericPool<T>
{
    protected readonly Queue<T> _objects = new Queue<T>();
    private readonly Func<T> _createFunc;

    protected GenericPool(Func<T> createFunc, int initialCount = 0)
    {
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        for (int i = 0; i < initialCount; i++)
            _objects.Enqueue(Create());
    }

    protected virtual T Create()
    {
        return _createFunc();
    }

    public virtual T Get()
    {
        return _objects.Count > 0 ? _objects.Dequeue() : Create();
    }

    public virtual void Return(T obj)
    {
        _objects.Enqueue(obj);
    }

    public void Clear()
    {
        _objects.Clear();
    }
}