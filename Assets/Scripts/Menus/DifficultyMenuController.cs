using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Elements")]
    public List<DifficultyButtonEntry> difficultyButtons;
    public CanvasGroup otherButtonGroup;

    public float buttonFadeDuration = 0.5f;
    public float descriptionFadeDuration = 0.5f;
    public float backButtonFadeDuration = 1f;

    [System.Serializable]
    public class DifficultyButtonEntry
    {
        public CanvasGroup buttonGroup;          // CanvasGroup on the button root (for fading)
        public CanvasGroup descriptionGroup;     // CanvasGroup for description text
        public Toggle toggle;                     // Toggle component for selection
    }

    private Coroutine fadeRoutine;

    public void OnEnableMenu()
    {
        gameObject.SetActive(true);

        foreach (var entry in difficultyButtons)
        {
            entry.buttonGroup.alpha = 0f;
            entry.descriptionGroup.alpha = 0f;
            entry.toggle.interactable = false;
        }

        otherButtonGroup.alpha = 0f;

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
        foreach (var entry in difficultyButtons)
        {
            yield return FadeCanvasGroup(entry.buttonGroup, buttonFadeDuration);
            entry.toggle.interactable = true;

            yield return FadeCanvasGroup(entry.descriptionGroup, descriptionFadeDuration);
        }

        yield return FadeCanvasGroup(otherButtonGroup, backButtonFadeDuration);
        otherButtonGroup.GetComponentInChildren<Selectable>().interactable = true;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float duration)
    {
        float elapsed = 0f;
        float startAlpha = cg.alpha;
        float endAlpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }
}
