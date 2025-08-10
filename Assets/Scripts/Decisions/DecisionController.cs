using System.Collections;
using UnityEngine;

public class DecisionController : MonoBehaviour
{
    [Header("Starting Point")]
    [SerializeField] private DecisionData _firstDecision;

    private DecisionData currentDecision;

    private float timeRemaining;

    Coroutine _cR;

    private void Awake()
    {
        SetNextDecision(_firstDecision);
        EventBus.Subscribe<EnterDecisionMakingState>( (_) => { StartDecision(); });
        //EventBus.Subscribe<ExitDecisionMakingState>();
    }
    public void InitializeDecision()
    {
        SetNextDecision(currentDecision);
    }

    public void SetNextDecision(DecisionData decision)
    {
        currentDecision = decision;
        timeRemaining = decision.timeLimitSeconds;

    }
    public void StartDecision()
    {     
        DecisionService.Instance.ShowDecisionUI(currentDecision);
        if (_cR != null) StopCoroutine(_cR);
        _cR = StartCoroutine(RunTimer());
    }

    private IEnumerator RunTimer()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DecisionService.Instance.UpdateTimer(currentDecision.timeLimitSeconds - timeRemaining);
            yield return null;
        }
        OnTimeExpired();
    }

    public void SelectChoice(int index)
    {
        if (currentDecision == null) return;

        if (_cR!=null) StopCoroutine(_cR);
        var choice = currentDecision.choices[index];
        ConsequenceRunner.Execute(choice.consequences);
    }

    private void OnTimeExpired()
    {
        if (_cR == null)
        {
#if UNITY_EDITOR
            Debug.Log("NO HAY CORUTINA PERO EL TIMER LLEGO AL FINAL");
#endif
            return;
        }

#if UNITY_EDITOR
        Debug.Log("Time expired! Applying default consequence.");
#endif
        StopCoroutine(_cR);

        // Could execute a default choice 
        ConsequenceRunner.Execute(currentDecision.defaultChoice.consequences);
    }
}
