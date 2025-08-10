using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DecisionMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Elements")]
    public CanvasGroup timerGroup;
    public CanvasGroup textGroup;
    public CanvasGroup buttonsGroup;

    [Header("Fade Durations")]
    public float timerFadeDuration = 0.5f;  // faster timer fade
    public float textFadeDuration = 1f;
    public float buttonsFadeDuration = 1f;

    private Coroutine fadeRoutine;

    public void OnEnableMenu()
    {
        gameObject.SetActive(true);
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    public void OnDisableMenu()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeInRoutine()
    {
        timerGroup.alpha = 0f;
        textGroup.alpha = 0f;
        buttonsGroup.alpha = 0f;
        SetButtonsInteractable(false);

        float elapsed = 0f;

        // Fade in timer first (faster)
        while (elapsed < timerFadeDuration)
        {
            elapsed += Time.deltaTime;
            timerGroup.alpha = Mathf.Clamp01(elapsed / timerFadeDuration);
            yield return null;
        }
        timerGroup.alpha = 1f;

        // Fade in text next
        elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            textGroup.alpha = Mathf.Clamp01(elapsed / textFadeDuration);
            yield return null;
        }
        textGroup.alpha = 1f;

        // Fade in buttons last
        elapsed = 0f;
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