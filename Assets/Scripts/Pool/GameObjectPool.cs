using UnityEngine;

public class GameObjectPool : GenericPool<GameObject>
{

    public GameObjectPool(GameObject prefab, int initialCount = 0, Transform parent = null)
        : base(() => CreateInstance(prefab, parent), initialCount)
    {
    }

    private static GameObject CreateInstance(GameObject prefab, Transform parent)
    {
        var obj = GameObject.Instantiate(prefab, parent);
        obj.SetActive(false); // Ensure the object is inactive when first created
        return obj;
    }

    public override GameObject Get()
    {
        var obj = base.Get();
        obj.SetActive(true); // Activate the object when it is retrieved from the pool
        return obj;
    }

    public override void Return(GameObject obj)
    {
        obj.SetActive(false); // Deactivate the object when it is returned to the pool
        base.Return(obj); // Return to the pool
    }
}
