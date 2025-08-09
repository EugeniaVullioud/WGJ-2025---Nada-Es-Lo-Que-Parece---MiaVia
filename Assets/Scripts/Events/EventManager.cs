using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private PoolManager poolManager = new PoolManager();
    void GenerateEvent<T>(Func<T> createInstance) where T: struct
    {
        // Get the pool for the specific event type
        var pool = poolManager.GetPool<T>(createInstance);

        // Get a pooled struct or create a new one
        var eventData = pool.Get();

        // Publish the event (for simplicity, let's assume EventBus handles it)
        EventBus.Publish(eventData);

        // Return the struct to the pool after use
        pool.Return(eventData);
    }

    public void OnEasyDifficultyChosen() => GenerateEvent(() => new DifficultyChosen( DifficultyTypes.Easy));
    public void OnMediumDifficultyChosen() => GenerateEvent(() => new DifficultyChosen(DifficultyTypes.Medium));
    public void OnHardDifficultyChosen() => GenerateEvent(() => new DifficultyChosen(DifficultyTypes.Hard));


}
