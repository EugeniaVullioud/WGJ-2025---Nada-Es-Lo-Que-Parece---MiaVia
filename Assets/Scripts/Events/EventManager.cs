using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    private void Awake() => Instance = this;

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

    #region Difficulty
    public void OnEasyDifficultyChosen() => GenerateEvent(() => new DifficultyChosen( DifficultyTypes.Easy));
    public void OnMediumDifficultyChosen() => GenerateEvent(() => new DifficultyChosen(DifficultyTypes.Medium));
    public void OnHardDifficultyChosen() => GenerateEvent(() => new DifficultyChosen(DifficultyTypes.Hard));
    #endregion

    #region Progress Bars
    public void OnModifyShameStat(float amount) => GenerateEvent(() => new ModifyShameStat(amount));
    public void OnModifyDistanceStat(float amount) => GenerateEvent(() => new ModifyDistanceTimeStat(amount));
    public void OnModifyDecisionTimerStat(float amount) => GenerateEvent(() => new ModifyDecisionTimeStat(amount));
    #endregion

    #region Decisions
    public void OnSetNextDecision(DecisionData amount) => GenerateEvent(() => new SetNextDecision(amount));
    public void OnTriggerNextDecision() => GenerateEvent(() => new TriggerNextDecision());


    #endregion
}
