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
    private Vector3 _initialScale;

    public MovingFadingObject PrefabReference { get; private set; }

    public void SetPrefabReference(MovingFadingObject prefab)
    {
        PrefabReference = prefab;
    }
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _startColor = _renderer.material.color;
    }

    public void InitMovement(Vector3 startPos, Vector3 endPos, float travelTime, System.Action<MovingFadingObject> onFinished, float maxScaleMultiplier = 1f,
                             float growthStartPercent = 0.7f)
    {
        _startPos = startPos;
        _endPos = endPos;
        _travelTime = travelTime;
        _elapsedTime = 0f;
        _onFinished = onFinished;

        _isActive = true;
        gameObject.SetActive(true);

        _initialScale = transform.localScale;

        _maxScaleMultiplier = maxScaleMultiplier;
        _growthStartPercent = growthStartPercent;
        transform.position = _startPos;
        transform.localScale = _initialScale;
    }

    private void Update()
    {
        if (!_isActive)
            return;

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
            float fadeOutTime = _elapsedTime - (_travelTime - _fadeDuration);
            c.a = Mathf.Lerp(_startColor.a, 0f, fadeOutTime / _fadeDuration);
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
            _onFinished?.Invoke(this);
        }
    }
    public void ResetObject()
    {
        transform.localScale = Vector3.one;
    }
}

