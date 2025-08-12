using System;
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

    private void Start()
    {
        EventBus.Subscribe<EnterDecisionMakingState>((e) => { Pause(); });
        EventBus.Subscribe<LostGame>((e) => {
            modifier = periodicEventIntervalModifier;
            time = periodicEventInterval; Pause(); });
        EventBus.Subscribe<WonGame>((e) => {
            modifier = periodicEventIntervalModifier;
            time = periodicEventInterval; Pause(); });
        EventBus.Subscribe<FinishDecisionMaking>((e) => { Resume(); });
        EventBus.Subscribe<PlayAgain>((e) => {
            modifier = periodicEventIntervalModifier;
            time = periodicEventInterval; Resume(); });
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
            _isPaused = true;
        }
    }

    public void Resume()
    {
        if (_isPaused)
        {
            Debug.Log(" despausa");
           

            _periodicTimer = 0f;
            _isPaused = false;
            //EventManager.Instance.OnFinishDecisionMaking();
        }
    }

    public void SetSpeed(float newSpeed)
    {
        _currentSpeed = newSpeed;
        EventManager.Instance.OnChangeSpeed();
    }

    public float GetCurrentSpeed() => _currentSpeed;

    private void Update()
    {

        // Only run periodic event if not paused
        if (!_isPaused)
        {
            _periodicTimer += Time.deltaTime;
            if (_periodicTimer > 0 && _periodicTimer < 0.4f) EventManager.Instance.OnTriggerNextDecision();

             if (_periodicTimer >= time)
            {
                Debug.Log(" deberia triggerear el spawn");

                _periodicTimer = 0f;
                EventManager.Instance.OnSpawnNextDecision();

            }
        }
    }
}
