using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    private bool _isPaused = false;
    private float _currentSpeed = 8f;

    [SerializeField] private float periodicEventInterval = 8f; // X seconds between events
    [SerializeField] private float periodicEventIntervalModifier = 1f; // X seconds between events


    private float _periodicTimer = 0f;

    private void Start()
    {
        EventBus.Subscribe<EnterDecisionMakingState>((e) => { Pause(); });
        EventBus.Subscribe<LostGame>((e) => { Pause(); });
        EventBus.Subscribe<WonGame>((e) => { Pause(); });
        EventBus.Subscribe<FinishDecisionMaking>((e) => { Resume(); });
        EventBus.Subscribe<PlayAgain>((e) => { Resume(); });

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


        if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            EventManager.Instance.OnTriggerNextDecision();


        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {

            Resume();

        }

        // Only run periodic event if not paused
        if (!_isPaused)
        {
            _periodicTimer += Time.deltaTime;

            if (_periodicTimer >= periodicEventInterval)
            {
                _periodicTimer = 0f;
                EventManager.Instance.OnTriggerNextDecision();

            }
        }
    }
}
