using UnityEngine;

public class DecisionService : MonoBehaviour
{
   
    public static DecisionService Instance { get; private set; }
    private void Awake() => Instance = this;

    public void ShowDecisionUI(DecisionData decision) 
    { 
        /* update UI */
    }
    public void UpdateTimer(float timeRemaining) 
    {
        /* update UI */
        EventManager.Instance.OnModifyDecisionTimerStat(timeRemaining);
    }
    public void ShowMessage(string message) 
    { 
        /* show popup */
    }
}
