using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditsMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Elements")]
    public CanvasGroup textGroup;
    public CanvasGroup buttonsGroup;

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
        textGroup.alpha = 0f;
        buttonsGroup.alpha = 0f;
        SetButtonsInteractable(false);

        float elapsed = 0f;
        // Fade in text first
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