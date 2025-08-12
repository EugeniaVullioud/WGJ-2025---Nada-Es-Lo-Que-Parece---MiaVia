using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ShameBarVisual : MonoBehaviour
{
    [SerializeField] private Image[] _shameSlider;
    [SerializeField] private float fillSpeed = 2f;
    [SerializeField] private float shakeStrenght = 2f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private Image _shake;

    Quaternion originalRot;

    private float _currentFill = 0f; // 0 to 1
    private float _lastCheck = -1f;

    private void UpdateFill(float value)
    {
        _currentFill = Mathf.Clamp01(value);

        // Fire events only when crossing thresholds
        if (_lastCheck < 0f) _lastCheck = _currentFill; // init

        if (_currentFill == 0f && _lastCheck > 0f) EventManager.Instance.OnEmptyShame();
        else if (_currentFill >= 1f && _lastCheck < 1f) { }
           
        else if (_currentFill >= 0.5f && _lastCheck < 0.5f) EventManager.Instance.OnEmptyShame();

        _lastCheck = _currentFill;
    }
    private ProgressBarStatHandler _shameStatHandler;

    private void Awake()
    {
        originalRot = _shake.transform.localRotation;
    }
    private void Start()
    {
        var shameStat = new Stat<float>("Shame", 100f, EventManager.Instance.OnLost, 0f);
        _shameStatHandler = new ProgressBarStatHandler(shameStat, _shameSlider);
        _shameStatHandler.UpdateProgressBars();

        EventBus.Subscribe<ModifyShameStat>((stat) => Modify(stat.Amount));
        EventBus.Subscribe<PlayAgain>((stat) => { _shameStatHandler.ResetStats(); UpdateFill(0); });
        EventBus.Subscribe<WonGame>((stat) => {
            _shameStatHandler.ResetStats(); UpdateFill(0);
            StopAllCoroutines();
        });
        EventBus.Subscribe<LostGame>((stat) => {
            _shameStatHandler.ResetStats(); UpdateFill(0);
            StopAllCoroutines();
        });

    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ModifyShameStat>((stat) => Modify(stat.Amount));
        EventBus.Unsubscribe<PlayAgain>((stat) => { _shameStatHandler.ResetStats(); UpdateFill(0); });
        EventBus.Unsubscribe<WonGame>((stat) => {
            _shameStatHandler.ResetStats(); UpdateFill(0);
            StopAllCoroutines();
        });
        EventBus.Unsubscribe<LostGame>((stat) => {
            _shameStatHandler.ResetStats(); UpdateFill(0);
            StopAllCoroutines();
        });
    }
    void Modify(float amount)
    {
        _shameStatHandler.ModifyStat(amount);
        UpdateFill(_shameStatHandler.Current);
        AnimateFillTo(_shameStatHandler, amount);
        if (amount > 0) Shake(shakeDuration, shakeStrenght);
    }
    Coroutine animate;

    public void AnimateFillTo( ProgressBarStatHandler handler, float target)
    {
        if (animate != null) StopCoroutine(animate);
        animate = StartCoroutine(FillRoutine( handler, Mathf.Clamp01(target)));
    }

    private IEnumerator FillRoutine( ProgressBarStatHandler handler, float target)
    {
        while (!Mathf.Approximately(handler.Current, target))
        {
            float newValue = Mathf.MoveTowards(handler.Current, target, fillSpeed * Time.deltaTime);
            for (int i = 0; i < _shameSlider.Length; i++)
            {
                // Update the slider value based on the current value of the stat
                _shameSlider[i].fillAmount = newValue;
            }
            yield return null;

        }


    }
    Coroutine shake;
    public void Shake(float duration, float strength)
    {
        if (shake!=null) StopCoroutine(shake);
        shake = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Sin(elapsed * 40f) * strength; // 40 = speed
            transform.localRotation = originalRot * Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        transform.localRotation = originalRot;
    }


    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            EventManager.Instance. OnModifyShameStat(10);

        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            EventManager.Instance.OnModifyShameStat(-5);

        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            EventManager.Instance.OnEndingMenuChosen();

    }
}
