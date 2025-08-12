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
    }
    private void Start()
    {
        _cam = Camera.main;
        EventBus.Subscribe<EnterDecisionMakingState>((_) => { StartDecision(); });
        EventBus.Subscribe<TriggerNextDecision>((_) => { Continue(); });
        EventBus.Subscribe<SetNextDecision>((e) => { SetNextDecision(e.NextDecision); });
        
        EventBus.Subscribe<PlayAgain>((e) => { ResetController(); _playerLost = false; _playerWon = false; });
        EventBus.Subscribe<LostGame>((e) => { OnPlayerLose(); });
        EventBus.Subscribe<OnCorrectlyAnswered>((e) => { if (e.WasCorrect) CorrectlyAnswered(); else IncorrectlyAnswered(); });

        _sequenceGenerator.GenerateSequence();

        timeBase = _initialTime;

        timeRemaining = timeBase;
    }

    private void ResetController()
    {
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
    void PlaceInCenter(Transform t)
    {
        // Get the screen center position (middle of the screen)
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, zoomDistance);

        // Convert screen center position to world space using the camera
        Vector3 worldPosition = _cam.ScreenToWorldPoint(screenCenter);

        // Set the object's position to that world position
        t.position = worldPosition;
    }

    public void OnPlayerLose()
    {
        _playerLost = true;
    }
    private Dictionary<string, GameObjectPool> _pools = new();

    public void SetNextDecision(DecisionData decision)
    {
        Debug.Log("SetNextDecision " + decision);

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

        PlaceInCenter(zoom.transform);

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
        Debug.Log("StartDecision");
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

        Invoke("Continue", 0.2f);
    }
    void Continue()
    {
        if (_playerLost) return;
        Debug.Log("Continue");

        var nextDecision = _sequenceGenerator.GetNextDecision();
        SetNextDecision(nextDecision);
    }
    private void OnTimeExpired()
    {
       

#if UNITY_EDITOR
        Debug.Log("Time expired! Applying default consequence.");
#endif
        if (_cR != null) StopCoroutine(_cR);


        // Could execute a default choice 
        ConsequenceRunner.Execute(currentDecision.defaultChoice.consequences);

        if (!_playerLost)Invoke("Continue", 0.2f);

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
