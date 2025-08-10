using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenuController : FadeInMenuBase
{
    [Header("UI Elements")]
    public CanvasGroup[] textGroups; // multiple text blocks fading sequentially
    public CanvasGroup buttonsGroup;

    public float textFadeDuration = 1f;
    public float buttonsFadeDuration = 1f;

    // Override FadeSequence to only fade buttons, since texts handled custom below
    protected override (CanvasGroup group, float duration)[] FadeSequence => new[]
    {
        (buttonsGroup, buttonsFadeDuration)
    };

    private Coroutine fadeRoutine;

    public override void OnEnableMenu()
    {
        gameObject.SetActive(true);
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTextsThenButtons());
    }

    public override void OnDisableMenu()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeTextsThenButtons()
    {
        DisableAllButtons();
        buttonsGroup.alpha = 0f;
        // Init all text alphas to 0
        foreach (var cg in textGroups)
            cg.alpha = 0f;

        // Fade texts one by one
        foreach (var cg in textGroups)
        {
            yield return FadeCanvasGroup(cg, textFadeDuration);
        }

        // Fade buttons last (using base fade method)
        yield return FadeCanvasGroup(buttonsGroup, buttonsFadeDuration);

        EnableAllButtons();
    }

    // Copy from base (or move to utility)
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

    protected override void EnableAllButtons()
    {
        foreach (Selectable btn in buttonsGroup.GetComponentsInChildren<Selectable>())
            btn.interactable = true;
    }

    protected override void DisableAllButtons()
    {
        foreach (Selectable btn in buttonsGroup.GetComponentsInChildren<Selectable>())
            btn.interactable = false;
    }
}