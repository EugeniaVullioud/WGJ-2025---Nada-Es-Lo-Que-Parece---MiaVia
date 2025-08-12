using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionService : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup group;

    [SerializeField] private float _displayDuration =3;
    [SerializeField] private float _animationDuration =1;

    public static DecisionService Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        group.alpha = 0;
        group.interactable = false;

        EventBus.Subscribe<PlayAgain>((stat) => { interruptUI = true; });
        EventBus.Subscribe<WonGame>((stat) => { interruptUI = true; });
        EventBus.Subscribe<PlayAgain>((stat) => { interruptUI = false; });
        }
    public void ShowDecisionUI(DecisionData decision) 
    { 
        /* update UI */
    }
    public void UpdateTimer(float timeRemaining) 
    {
        EventManager.Instance.OnModifyDecisionTimerStat(timeRemaining);
    }
    bool interruptUI = false;
    public void ShowMessage(string message) 
    {
        if (interruptUI)
        {
            StopAllCoroutines();
            group.alpha = 0;
            group.interactable = false;
            return;
        }
        _text.text = message;

        //animate appear

        StopAllCoroutines();
        StartCoroutine(AnimateAppear());
    }

    private IEnumerator AnimateAppear()
    {

        group.alpha = 0;
        float time = 0;

        while (time< 1)
        {
            // make background and text appear
            time += Time.deltaTime /_animationDuration;
            group.alpha = time;
            yield return null;
        }
        // animation to appear, wait 
        group.alpha = 1;
        group.interactable = true;

        yield return new WaitForSeconds(_displayDuration);
        group.alpha = 0;
        group.interactable = false;


    }
}
