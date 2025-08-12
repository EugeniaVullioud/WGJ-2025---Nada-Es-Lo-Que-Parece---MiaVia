using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    private bool _isPaused = false;
    private float _currentSpeed = 8f;

    [SerializeField] private float periodicEventInterval = 8f; // X seconds between events
    [SerializeField] private float periodicEventIntervalModifier = 1f; // X seconds between events

    float modifier;
    float time;

    private float _periodicTimer = 0f;
    private bool start = true;

    private void Start()
    {
        EventBus.Subscribe<EnterDecisionMakingState>((e) => { Pause(); });
        EventBus.Subscribe<MenuChosen>((e) => { if (MenuType.MainMenu == e.Menu) start = true; });
        EventBus.Subscribe<LostGame>((e) => {
            modifier = periodicEventIntervalModifier;
            time = periodicEventInterval; Pause(); });
        EventBus.Subscribe<WonGame>((e) => {
            modifier = periodicEventIntervalModifier;
            time = periodicEventInterval; Pause(); });
        EventBus.Subscribe<FinishDecisionMaking>((e) => { Resume(); });
        EventBus.Subscribe<PlayAgain>((e) => {
            modifier = periodicEventIntervalModifier;
            Resume(); 
            if (start) { 
                time = 1; start = false; }
            else { time = periodicEventInterval; } 
        
        });
        EventBus.Subscribe<OnCorrectlyAnswered>((e) => { if (time < 1) return;
            if (e.WasCorrect) 
            { time -= 0.2f; } 
            else { time -= 0.4f; }; });

        _isPaused = true;
    }
    public void Pause()
    {
        if (!_isPaused)
        {
            _ensureStack = false;

            _isPaused = true;
        }
    }

    public void Resume()
    {
        if (_isPaused)
        {
            //Debug.Log(" despausa");
           

            _periodicTimer = 0f;
            _isPaused = false;

            if (!_ensureStack)
            {
                _ensureStack = true;
                EventManager.Instance.OnCheckPendingSpawn();
                //Debug.Log(" despausa OnSpawnNextDecision");

                EventManager.Instance.OnSpawnNextDecision();
            }
           

        }
    }

    public void SetSpeed(float newSpeed)
    {
        _currentSpeed = newSpeed;
        EventManager.Instance.OnChangeSpeed();
    }

    public float GetCurrentSpeed() => _currentSpeed;

    bool _preparedNext = false;
    bool _ensureStack = false;
    private void Update()
    {
        if (_isPaused && !_ensureStack)
        {
            
                //Debug.Log(" _ensureStack deberia triggerear el spawn");

                _periodicTimer = 0f;
                EventManager.Instance.OnSpawnNextDecision();
                _ensureStack = true;

        }

        // Only run periodic event if not paused
        if (!_isPaused)
        {
            _periodicTimer += Time.deltaTime;
            if (_periodicTimer > 0 && _periodicTimer < 0.4f && !_preparedNext)
            {
                _preparedNext=true;
                EventManager.Instance.OnTriggerNextDecision();
            }

             if (_periodicTimer >= time)
            {
                if (!start) time = periodicEventInterval;
                _periodicTimer = 0f;
                _preparedNext = false;
                EventManager.Instance.OnSpawnNextDecision();

            }
        }
    }
}
