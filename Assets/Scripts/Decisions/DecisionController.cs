using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionController : MonoBehaviour
{
    private bool _playerLost = false;
    private bool _playerWon = false;

    public static int InEventQueue = 0;
    public bool inEventMode = false;

    [Header("Starting Point")]
    [SerializeField] private DecisionSequenceGenerator _sequenceGenerator;

    private DecisionData currentDecision;

    [Header("Time")]
    [SerializeField] private float _initialTime = 5;
    [SerializeField] private float _correctTimeLose = 0.25f;
    [SerializeField] private float _incorrectTimeLose = 0.5f;
    
    private float timeRemaining;
    private float timeBase;

    Coroutine _cR;

    [Header("UI Elements")]
    [SerializeField] private Button _button1;
    [SerializeField] private TMP_Text _button1Text;
    [SerializeField] private Button _button2;
    [SerializeField] private TMP_Text _button2Text;
    [SerializeField] private Image comparison;
    [SerializeField] private GameObject zoom;
    [SerializeField] private float zoomDistance=200;

    [SerializeField] private TMP_Text _textField; // Use TextMeshProUGUI if using TextMeshPro

    private void Awake()
    {
        _button1Text = _button1.GetComponentInChildren<TMP_Text>();
        _button2Text = _button2.GetComponentInChildren<TMP_Text>();


        _button1.onClick.AddListener(() => SelectChoice(0));
        _button2.onClick.AddListener(() => SelectChoice(1));

        //SetNextDecision(_firstDecision);

        //EventBus.Subscribe<ExitDecisionMakingState>();

        comparison.sprite = null;
    }
    private void Start()
    {
        _cam = Camera.main;
        EventBus.Subscribe<EnterDecisionMakingState>((_) => { StartDecision(); });
        EventBus.Subscribe<TriggerNextDecision>((_) => { Continue(); });
        EventBus.Subscribe<SetNextDecision>((e) => { SetNextDecision(e.NextDecision); });
        
        EventBus.Subscribe<PlayAgain>((e) => { ResetController(); _playerLost = false; _playerWon = false; inEventMode = false; InEventQueue = 0; _decisionQueue.Clear(); _sequenceGenerator.ResetQueue(); });
        EventBus.Subscribe<LostGame>((e) => { OnPlayerLose(); _playerWon = false;  inEventMode = false; InEventQueue = 0; _decisionQueue.Clear(); _sequenceGenerator.ResetQueue(); });
        EventBus.Subscribe<WonGame>((e) => { _playerWon = true;  inEventMode = false; InEventQueue = 0; _decisionQueue.Clear(); _sequenceGenerator.ResetQueue(); });
        EventBus.Subscribe<OnCorrectlyAnswered>((e) => { if (e.WasCorrect) CorrectlyAnswered(); else IncorrectlyAnswered(); });

        _sequenceGenerator.GenerateSequence();

        timeBase = _initialTime;

        timeRemaining = timeBase;
    }

    private void ResetController()
    {
        InEventQueue = 0;
        timeBase = _initialTime;
    }
    void CorrectlyAnswered()
    {
        timeBase -= _correctTimeLose;
    }
    void IncorrectlyAnswered()
    {
        timeBase -= _incorrectTimeLose;
    }

    Camera _cam;
    void PlaceInCenter(Transform t, float offsetPercentage)
    {
        // Get object's height from its Renderer bounds
        Renderer r = t.GetComponentInChildren<Renderer>();
        float objectHeight = r != null ? r.bounds.size.y : 0f;

        // We want to offset so the object appears centered by height
        float heightOffset = objectHeight * offsetPercentage;

        // The point we project is adjusted backwards by that height in local space
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, zoomDistance);

        // Convert to world space
        Vector3 worldCenter = _cam.ScreenToWorldPoint(screenCenter);

        // Move the object so its center is at that world position
        // But we subtract the offset upward so taller objects still center visually
        t.position = worldCenter - Vector3.up * heightOffset;
    }

    public void OnPlayerLose()
    {
        _playerLost = true;
    }
    private Dictionary<string, GameObjectPool> _pools = new();

    private Queue<DecisionData> _decisionQueue = new Queue<DecisionData>();


    public void SetNextDecision(DecisionData decision)
    {

        currentDecision = decision;
        timeRemaining = timeBase;

        if (!_pools.ContainsKey(currentDecision.onZoom.name))
        {
            var pool = new GameObjectPool(currentDecision.onZoom, 3, transform);
            _pools[currentDecision.onZoom.name] = pool;
        }
        var obj = _pools[currentDecision.onZoom.name].Get();

        zoom = obj;
    
       zoom.transform.rotation = Quaternion.Euler(90, 180, 0);

        PlaceInCenter(zoom.transform, decision.verticalOffsetPercentage);

        zoom.SetActive(false);


        if (decision.compare != null)
        {
            comparison.gameObject.SetActive(true);

            comparison.sprite = decision.compare.sprite;
        }
        UpdateUIText(currentDecision.prompt);

        UpdateButtonText(_button1Text, currentDecision.choices[0].choiceText);
        UpdateButtonText(_button2Text, currentDecision.choices[1].choiceText);


    }
    public void StartDecision()
    {
        if (_playerLost) return;

        if (_decisionQueue.Count == 0)
        {
            if (InEventQueue > 0 && !inEventMode)
            {
                Continue();
            }
            else
            {

                return;
            }
        }
        inEventMode = true;
        var decisionToStart = _decisionQueue.Dequeue();
        SetNextDecision(decisionToStart);
        InEventQueue--;

        zoom.SetActive(true);

        //DecisionService.Instance.ShowDecisionUI(currentDecision);
        if (_cR != null) StopCoroutine(_cR);
        _cR = StartCoroutine(RunTimer());


       
    }
    // Method to get the correct pool from the dictionary
    private GameObjectPool GetZoomPool(string poolKey, GameObject gm)
    {
        if (!_pools.ContainsKey(poolKey))
        {
            // If the pool doesn't exist, create a new one and add it to the dictionary
            _pools[poolKey] = new GameObjectPool(gm, 10);
        }
        return _pools[poolKey];
    }

    // Return zoom object to the pool when done
    public void ReturnZoomObject(GameObject zoomInstance, string poolKey)
    {
        if (_pools.ContainsKey(poolKey))
        {
            _pools[poolKey].Return(zoomInstance);  // Return to the correct pool
        }
    }

    private IEnumerator RunTimer()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DecisionService.Instance.UpdateTimer(1-timeRemaining/currentDecision.timeLimitSeconds);
            yield return null;
        }
        if (zoom != null)
        {
            _pools[currentDecision.onZoom.name].Return(zoom);
            zoom.SetActive(false);
        }
        comparison.sprite = null;

        comparison.gameObject.SetActive(false);

        EventManager.Instance.OnFinishDecisionMaking();

        yield return null;

        OnTimeExpired();
    }

    public void SelectChoice(int index)
    {
        if (currentDecision == null) return;

        if (_cR!=null) StopCoroutine(_cR);
        _pools[currentDecision.onZoom.name].Return(zoom);
        zoom.SetActive(false);

        comparison.sprite = null;
        comparison.gameObject.SetActive(false);

        var choice = currentDecision.choices[index];

        EventManager.Instance.OnFinishDecisionMaking();
        ConsequenceRunner.Execute(choice.consequences);
        inEventMode = false;
        Invoke("Continue", 0.2f);
    }
    void Continue()
    {
        if (_playerLost || _playerWon) return;

        var nextDecision = _sequenceGenerator.GetNextDecision();

        EventManager.Instance.OnQueueNext(nextDecision);


        _decisionQueue.Enqueue(nextDecision); // Store for later

    }
    private void OnTimeExpired()
    {
       

#if UNITY_EDITOR
        Debug.Log("Time expired! Applying default consequence.");
#endif
        if (_cR != null) StopCoroutine(_cR);


        // Could execute a default choice 
        ConsequenceRunner.Execute(currentDecision.defaultChoice.consequences);
        inEventMode = false;
        if (!_playerLost)Invoke("Continue", 0.2f);

    }
    private void Update()
    {
        if (InEventQueue > 0 && !inEventMode)
        {

            EventManager.Instance.OnEnterDecisionMaking();
        }
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
