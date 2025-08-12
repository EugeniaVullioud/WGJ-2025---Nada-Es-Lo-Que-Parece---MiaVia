using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShameBarVisual : MonoBehaviour
{
    [SerializeField] private Image[] _shameSlider;
    [SerializeField] private float fillSpeed = 2f;
    [SerializeField] private float shakeStrenght = 2f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private Image _shake;

    public CanvasGroup _cg;
    Quaternion originalRot;
    private float _targetFill;   // 0 to 1

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
        var shameStat = new Stat<float>("Shame", 100f, () => { EventManager.Instance.OnLost(false); } , 0f);
        _shameStatHandler = new ProgressBarStatHandler(shameStat, _shameSlider);
        _shameStatHandler.UpdateProgressBars();

        EventBus.Subscribe<ModifyShameStat>((stat) => Modify(stat.Amount));
        EventBus.Subscribe<PlayAgain>((stat) => { _shameStatHandler.ResetStats(); _currentFill = 0; _targetFill = 0; UpdateFill(0); _cg.alpha = 1; });
        EventBus.Subscribe<WonGame>((stat) => {
            _currentFill = 0; _targetFill = 0;
            _shameStatHandler.ResetStats(); UpdateFill(0);
            StopCoroutine(shake);
            _shake.transform.localRotation = originalRot;
        });
        EventBus.Subscribe<LostGame>((stat) => {
            _currentFill = 0; _targetFill = 0;
            _shameStatHandler.ResetStats(); UpdateFill(0);
            StopCoroutine(shake);
            _shake.transform.localRotation = originalRot;

        });
        EventBus.Subscribe<MenuChosen>((stat) =>
        {
            if (stat.Menu == MenuType.InGameMenu || stat.Menu == MenuType.DecisionMenu) _cg.alpha = 1;
            else _cg.alpha = 0;
        });
            StartCoroutine(FillUpdater());

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
    private IEnumerator FillUpdater()
    {
        while (true)
        {
            if (!Mathf.Approximately(_currentFill, _targetFill))
            {
                _currentFill = Mathf.MoveTowards(_currentFill, _targetFill, fillSpeed * Time.deltaTime);

                for (int i = 0; i < _shameSlider.Length; i++)
                    _shameSlider[i].fillAmount = _currentFill;

                UpdateFill(_currentFill); // fire events when crossing thresholds
            }

            yield return null;
        }
    }

    void Modify(float amount)
    {
        _shameStatHandler.ModifyStat(amount);
        _targetFill = Mathf.Clamp01(_shameStatHandler.Current);

        if (amount > 0)
            Shake(shakeDuration, shakeStrenght);
    }
    Coroutine animate;


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

}
