using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionController : MonoBehaviour
{
    [Header("Starting Point")]
    [SerializeField] private DecisionData _firstDecision;

    private DecisionData currentDecision;

    private float timeRemaining;

    Coroutine _cR;

    [Header("UI Elements")]
    [SerializeField] private Button _button1;
    [SerializeField] private TMP_Text _button1Text;
    [SerializeField] private Button _button2;
    [SerializeField] private TMP_Text _button2Text;

    [SerializeField] private TMP_Text _textField; // Use TextMeshProUGUI if using TextMeshPro

    private void Awake()
    {
        _button1Text = _button1.GetComponentInChildren<TMP_Text>();
        _button2Text = _button2.GetComponentInChildren<TMP_Text>();


        _button1.onClick.AddListener(() => SelectChoice(0));
        _button2.onClick.AddListener(() => SelectChoice(1));

        SetNextDecision(_firstDecision);

        //EventBus.Subscribe<ExitDecisionMakingState>();
    }
    private void Start()
    {
        EventBus.Subscribe<EnterDecisionMakingState>((_) => { StartDecision(); });
        EventBus.Subscribe<SetNextDecision>((e) => { SetNextDecision(e.NextDecision); });

    }
    public void SetNextDecision(DecisionData decision)
    {
        UpdateUIText($"SetNextDecision {decision} selected.");

        currentDecision = decision;
        timeRemaining = decision.timeLimitSeconds;

        UpdateUIText(currentDecision.prompt);

        UpdateButtonText(_button1Text, currentDecision.choices[0].choiceText);
        UpdateButtonText(_button2Text, currentDecision.choices[1].choiceText);
    }
    public void StartDecision()
    {     
        //DecisionService.Instance.ShowDecisionUI(currentDecision);
        if (_cR != null) StopCoroutine(_cR);
        _cR = StartCoroutine(RunTimer());
    }

    private IEnumerator RunTimer()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DecisionService.Instance.UpdateTimer(1-timeRemaining/currentDecision.timeLimitSeconds);
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

        EventManager.Instance.OnFinishDecisionMaking();

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


    #region Match UI
    private void UpdateUIText(string message)
    {
        
        _textField.text = message;
        
    }
    private void UpdateButtonText(TMP_Text button, string message)
    {
        button.text = message;       
    }
    #endregion
}
