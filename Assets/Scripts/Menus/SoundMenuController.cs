using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Elements")]
    public CanvasGroup slidersGroup;
    public CanvasGroup textGroup;
    public CanvasGroup buttonsGroup;

    public float slidersFadeDuration = 1f;
    public float textFadeDuration = 1f;
    public float buttonsFadeDuration = 1f;

    private Coroutine fadeRoutine;

    public void OnEnableMenu()
    {
        gameObject.SetActive(true);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    public void OnDisableMenu()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeInRoutine()
    {
        slidersGroup.alpha = 0f;
        textGroup.alpha = 0f;
        buttonsGroup.alpha = 0f;
        SetButtonsInteractable(false);

        float elapsed = 0f;
        // Fade in sliders first
        while (elapsed < slidersFadeDuration)
        {
            elapsed += Time.deltaTime;
            slidersGroup.alpha = Mathf.Clamp01(elapsed / slidersFadeDuration);
            yield return null;
        }
        slidersGroup.alpha = 1f;

        elapsed = 0f;
        // Then fade in text
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            textGroup.alpha = Mathf.Clamp01(elapsed / textFadeDuration);
            yield return null;
        }
        textGroup.alpha = 1f;

        elapsed = 0f;
        // Then fade in buttons
        while (elapsed < buttonsFadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonsGroup.alpha = Mathf.Clamp01(elapsed / buttonsFadeDuration);
            yield return null;
        }
        buttonsGroup.alpha = 1f;
        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool enabled)
    {
        foreach (Selectable btn in buttonsGroup.GetComponentsInChildren<Selectable>())
        {
            btn.interactable = enabled;
        }
    }
}


