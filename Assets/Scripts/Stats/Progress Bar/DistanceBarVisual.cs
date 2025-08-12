using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class DistanceBarVisual : MonoBehaviour
{
    public RectTransform uiBarRectTransform;  // The RectTransform of the UI Image (UI Bar)
    public RectTransform targetRectTransform; // The RectTransform of the moving visual
    public RectTransform fillTransform; // The RectTransform of the moving visual
    public float moveDuration = 3f; // Time it takes to move from left to right
    public float startOffset = 0f; // Optional offset to start from (can be adjusted in the inspector)
    public float endOffset = 0f; // Optional offset to start from (can be adjusted in the inspector)
    public CanvasGroup _cg;

    [SerializeField] private float fillSpeed = 2f;

    private Vector2 startPosition;
    private Vector2 endPosition;

    private ProgressBarStatHandler _distanceStatHandler;

    void Awake()
    {
        mod = -1;
        // Get the starting position (left edge of the UI Bar + optional offset)
        startPosition = new Vector2(uiBarRectTransform.rect.xMin + startOffset, targetRectTransform.anchoredPosition.y);

        // Get the ending position (right edge of the UI Bar)
        endPosition = new Vector2(uiBarRectTransform.rect.xMax + endOffset, targetRectTransform.anchoredPosition.y);

        // Set the initial position of the moving visual
        targetRectTransform.anchoredPosition = startPosition;

    }

    private void Start()
    {
        var distanceStat = new Stat<float>("Distance", 100f, EventManager.Instance.OnWonGame, 0f);

        Image[] im = new Image[1];
        im[0] = fillTransform.GetComponent<Image>();

        _distanceStatHandler = new ProgressBarStatHandler(distanceStat, im);
        _distanceStatHandler.UpdateProgressBars();

        EventBus.Subscribe<ModifyDistanceTimeStat>((stat) => Modify(stat.Amount / 100));
        EventBus.Subscribe<WonGame>((stat) => {
            progress = 0;
            target = 0;
            mod = -1;
            _distanceStatHandler.ResetStats();
            StopAllCoroutines();
        });
        EventBus.Subscribe<LostGame>((stat) => {
            progress = 0;
            target = 0;
            mod = -1;
            _distanceStatHandler.ResetStats();
            StopAllCoroutines();
        });
        EventBus.Subscribe<PlayAgain>((stat) => {
            progress = 0;
            target = 0;
            mod = -1;
            _cg.alpha = 1;
            _distanceStatHandler.ResetStats();
            StopAllCoroutines();
            targetRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, 0);
        });
        EventBus.Subscribe<MenuChosen>((stat) =>
        {
            if (stat.Menu == MenuType.InGameMenu || stat.Menu == MenuType.DecisionMenu) _cg.alpha = 1;
            else _cg.alpha = 0;
        });
    }

    private void OnEnable()
    {
        if (mod != -1)
        {
            Modify(mod);
            mod = -1;
        }
    }

    private void OnDestroy ()
    {
        EventBus.Unsubscribe<ModifyDistanceTimeStat>((stat) => Modify(stat.Amount/100)); 
        EventBus.Unsubscribe<WonGame>((stat) => {
            progress = 0;
            target = 0;
            mod = -1;
            _distanceStatHandler.ResetStats();
            StopAllCoroutines();
        });
        EventBus.Unsubscribe<LostGame>((stat) => {
            progress = 0;
            target = 0;
            mod = -1;
            _distanceStatHandler.ResetStats();
            StopAllCoroutines();
        });
        EventBus.Unsubscribe<PlayAgain>((stat) => {
            progress = 0;
            target = 0;
            mod = -1;
            _distanceStatHandler.ResetStats();
            StopAllCoroutines();
            targetRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, 0);
        });
    }

    // Value between 0 and 1, representing the progress of the movement
    [Range(0, 101f)] public float progress = 0f;
    [Range(0, 101f)] public float target = 0f;

    void Update()
    {
        // Set the position based on progress (0 to 1)


        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            EventManager.Instance.OnModifyDistanceStat(10);

        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            EventManager.Instance.OnModifyDistanceStat(-5);

        }
    }
    float mod;
    void Modify(float amount)
    {
        if (!gameObject.activeSelf)
        {
            mod = amount;
            return;
        }
        else mod = -1;
            _distanceStatHandler.ModifyStat(amount * 100);
        if (animate != null)
        {
            StopCoroutine(animate);
            progress = target ;
           targetRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, progress);


        }
        target += amount;
        AnimateFillTo(_distanceStatHandler, progress + amount);
    }

    Coroutine animate;

    public void AnimateFillTo(ProgressBarStatHandler handler, float target)
    {

        if (_distanceStatHandler.Current == 1)
        {
            return;
        }
        Debug.Log("stat distance" + _distanceStatHandler.Current);
        mod = -1;
        animate = StartCoroutine(FillRoutine(handler, Mathf.Clamp01(target)));
    }

    private IEnumerator FillRoutine(ProgressBarStatHandler handler, float target)
    {
        yield return null; // Optionally use `yield return new WaitForEndOfFrame()` for a finer delay

        var add = progress;
        while (add< target)
        {

            add += fillSpeed * Time.deltaTime;
            targetRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, add);

            yield return null;

        }

        progress = target;

    }
}