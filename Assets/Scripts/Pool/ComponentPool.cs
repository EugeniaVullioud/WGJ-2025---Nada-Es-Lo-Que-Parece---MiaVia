using System.Collections.Generic;
using UnityEngine;

public class ComponentPool<T> : GenericPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;

    public ComponentPool(T prefab, int initialCount = 0, Transform parent = null)
        : base(() => CreateInstance(prefab, parent), initialCount)
    {
        _prefab = prefab;
        _parent = parent;
    }

    private static T CreateInstance(T prefab, Transform parent)
    {
        var obj = GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public override T Get()
    {
        var obj = base.Get();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public override void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        base.Return(obj);
    }
}