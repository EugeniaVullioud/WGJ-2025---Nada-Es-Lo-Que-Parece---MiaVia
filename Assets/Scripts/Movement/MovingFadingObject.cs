using UnityEngine;

public class MovingFadingObject : MonoBehaviour
{
    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _travelTime;
    private float _elapsedTime;
    private System.Action<MovingFadingObject> _onFinished;

    private Renderer _renderer;
    private Color _startColor;
    public float _fadeDuration = 1f; // seconds for fade in and fade out

    private bool _isActive = false;

    private float _maxScaleMultiplier = 1f;
    private float _growthStartPercent = 0.7f;
    [SerializeField]private Vector3 _initialScale;

    private bool _isMoving;
    private bool _isPaused;

    public MovingFadingObject PrefabReference { get; private set; }

    public void SetPrefabReference(MovingFadingObject prefab)
    {
        PrefabReference = prefab;
    }
    [HideInInspector]
    public int poolIndex;
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();

        // Create a unique instance of the material
        _renderer.material = new Material(_renderer.material);

        _startColor = _renderer.material.color;
    }
    private void OnEnable()
    {
        EventBus.Subscribe<EnterDecisionMakingState>((e) => { HandlePause(); });
        EventBus.Subscribe<LostGame>((e) => { HandlePause(); });
        EventBus.Subscribe<WonGame>((e) => { HandlePause(); });
        EventBus.Subscribe<FinishDecisionMaking>((e) => { HandleResume(); });
        EventBus.Subscribe<PlayAgain>((e) => { HandleResume(); });
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnterDecisionMakingState>((e) => { HandlePause(); });
        EventBus.Unsubscribe<FinishDecisionMaking>((e) => { HandleResume(); });
        EventBus.Unsubscribe<PlayAgain>((e) => { HandleResume(); });
        EventBus.Unsubscribe<LostGame>((e) => { HandlePause(); });
        EventBus.Unsubscribe<WonGame>((e) => { HandlePause(); });
    }
    public void InitMovement(Vector3 startPos, Vector3 endPos, float travelTime, System.Action<MovingFadingObject> onFinished, float maxScaleMultiplier = 1f,
                             float growthStartPercent = 0.7f, bool paused = false)
    {
        _startPos = startPos;
        _endPos = endPos;
        _travelTime = travelTime;
        _elapsedTime = 0f;
        _onFinished = onFinished;

        _isActive = true;
        _isMoving = true;
        _isPaused = paused;
        gameObject.SetActive(true);

        _initialScale = transform.localScale;

        _maxScaleMultiplier = maxScaleMultiplier;
        _growthStartPercent = growthStartPercent;
        transform.position = _startPos;
        transform.localScale = _initialScale;


        // Reset alpha
        Color reset = _startColor;
        reset.a = 0f;
        _renderer.material.color = reset;
    }

    private void Update()
    {
        if (!_isMoving || _isPaused) return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _travelTime);

        // Smoothly move from start to end
        transform.position = Vector3.Lerp(_startPos, _endPos, t);

        // Fade in/out alpha based on progress
        Color c = _startColor;
        if (_elapsedTime < _fadeDuration)
        {
            // Fade in
            c.a = Mathf.Lerp(0f, _startColor.a, _elapsedTime / _fadeDuration);
        }
        else if (_elapsedTime > _travelTime - _fadeDuration)
        {
            // Fade out
            //float fadeOutTime = _elapsedTime - (_travelTime - _fadeDuration);
            //c.a = Mathf.Lerp(_startColor.a, 0f, fadeOutTime / _fadeDuration);
        }
        else
        {
            c.a = _startColor.a;
        }
        _renderer.material.color = c;
        // Growth phase
        if (t >= _growthStartPercent)
        {
            float growthT = Mathf.InverseLerp(_growthStartPercent, 1f, t);
            float scale = Mathf.Lerp(1f, _maxScaleMultiplier, growthT);
            transform.localScale = _initialScale * scale;
        }
        // End of movement
        if (_elapsedTime >= _travelTime)
        {
            _isActive = false;
            _isMoving = false;

            _onFinished?.Invoke(this);
        }
    }

    public void ResetObject()
    {
        transform.localScale = _initialScale;
        _isMoving = false;
        _isPaused = false;

        Color resetColor = _startColor;
        resetColor.a = _startColor.a;
        _renderer.material.color = resetColor;
    }

    private void HandlePause() => _isPaused = true;
    private void HandleResume() => _isPaused = false;
}

