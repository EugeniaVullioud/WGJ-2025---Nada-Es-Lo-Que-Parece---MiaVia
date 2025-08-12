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

    MenuType menu;
    private void Start()
    {
        group.alpha = 0;
        group.interactable = false;

        EventBus.Subscribe<LostGame>((stat) => { interruptUI = true; Interrupt(); });
        EventBus.Subscribe<EnterDecisionMakingState>((stat) => { interruptUI = true; Interrupt(); });
        EventBus.Subscribe<FinishDecisionMaking>((stat) => { interruptUI = false; });
        EventBus.Subscribe<MenuChosen>((stat) => { menu = stat.Menu;  if ( MenuType.EndingMenu == stat.Menu || MenuType.LoseMenu == stat.Menu) { interruptUI = true; Interrupt(); } else interruptUI = false; });
        EventBus.Subscribe<WonGame>((stat) => { interruptUI = true; Interrupt(); });
        EventBus.Subscribe<PlayAgain>((stat) => { interruptUI = false; Interrupt(); });
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

    void Interrupt()
    {
        StopAllCoroutines();
        group.alpha = 0;
        group.interactable = false;
    }
    public void ShowMessage(string message) 
    {
      
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

        float t = 0;
        while (!interruptUI && (MenuType.DecisionMenu != menu || MenuType.EndingMenu != menu || MenuType.LoseMenu != menu) )
        {
            t+= Time.deltaTime ;
            yield return null;
        }
        group.alpha = 0;
        group.interactable = false;


    }
}
