using System.Collections;
using UnityEngine;

public class InGameMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Sliders")]
    public CanvasGroup[] sliderGroups;  // Assign each slider’s CanvasGroup here

    public float fadeDurationPerSlider = 0.5f;

    private Coroutine fadeRoutine;

    public void OnEnableMenu()
    {
        gameObject.SetActive(true);

        return;
        // Set all sliders invisible initially
        foreach (var cg in sliderGroups)   cg.alpha = 0f;

        fadeRoutine = StartCoroutine(FadeSlidersSequentially());
    }

    public void OnDisableMenu()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        gameObject.SetActive(false);
    }

    private IEnumerator FadeSlidersSequentially()
    {
        foreach (var cg in sliderGroups)
        {
            yield return FadeCanvasGroup(cg, fadeDurationPerSlider);
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }
}