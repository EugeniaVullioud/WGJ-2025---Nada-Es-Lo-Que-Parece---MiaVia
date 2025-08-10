using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoseMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Groups")]
    public CanvasGroup backgroundGroup;
    public CanvasGroup imagesGroup;
    public CanvasGroup textGroup;
    public CanvasGroup buttonsGroup;

    public float imagesFadeDuration = 1f;
    public float textFadeDuration = 1f;
    public float buttonsFadeDuration = 1f;

    private Coroutine fadeRoutine;

    public void OnEnableMenu()
    {
        gameObject.SetActive(true);

        imagesGroup.alpha = 0f;
        textGroup.alpha = 0f;
        buttonsGroup.alpha = 0f;

        SetButtonsInteractable(false);

        fadeRoutine = StartCoroutine(FadeSequenceRoutine());
    }

    public void OnDisableMenu()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        gameObject.SetActive(false);
    }

    private IEnumerator FadeSequenceRoutine()
    {
        yield return FadeCanvasGroup(backgroundGroup, imagesFadeDuration);
        yield return FadeCanvasGroup(imagesGroup, imagesFadeDuration);
        yield return FadeCanvasGroup(textGroup, textFadeDuration);
        yield return FadeCanvasGroup(buttonsGroup, buttonsFadeDuration);

        SetButtonsInteractable(true);
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

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Selectable btn in buttonsGroup.GetComponentsInChildren<Selectable>())
        {
            btn.interactable = interactable;
        }
    }
}