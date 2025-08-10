using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class FadeInMenuBase : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    /// <summary>
    /// Ordered list of CanvasGroups to fade in sequentially.
    /// </summary>
    protected abstract (CanvasGroup group, float duration)[] FadeSequence { get; }

    private Coroutine fadeRoutine;

    public virtual void OnEnableMenu()
    {
        gameObject.SetActive(true);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    public virtual void OnDisableMenu()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeInRoutine()
    {
        // Disable interaction on buttons until fully faded
        DisableAllButtons();

        // Initialize alphas to 0
        foreach (var (group, _) in FadeSequence)
        {
            group.alpha = 0f;
        }

        // Sequentially fade in each group
        foreach (var (group, duration) in FadeSequence)
        {
            yield return FadeCanvasGroup(group, duration);
        }

        EnableAllButtons();
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

    /// <summary>
    /// Enable all Selectable components in the last CanvasGroup (usually buttons)
    /// Override if you want to change behaviour.
    /// </summary>
    protected virtual void EnableAllButtons()
    {
        if (FadeSequence.Length == 0) return;
        var lastGroup = FadeSequence[^1].group;
        foreach (Selectable btn in lastGroup.GetComponentsInChildren<Selectable>())
        {
            btn.interactable = true;
        }
    }

    protected virtual void DisableAllButtons()
    {
        if (FadeSequence.Length == 0) return;
        var lastGroup = FadeSequence[^1].group;
        foreach (Selectable btn in lastGroup.GetComponentsInChildren<Selectable>())
        {
            btn.interactable = false;
        }
    }
}
