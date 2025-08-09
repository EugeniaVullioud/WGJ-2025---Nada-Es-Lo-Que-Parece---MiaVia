using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPool<T> : GenericPool<T> where T : struct
{
    public EventPool(Func<T> createFunc, int initialCount = 0)
        : base(createFunc, initialCount)
    {
    }
}