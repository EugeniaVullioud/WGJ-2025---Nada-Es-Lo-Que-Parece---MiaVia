using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour, IMenuController
{
    public GameObject Root => gameObject;

    [Header("UI Elements")]
    public CanvasGroup backgroundGroup;
    public CanvasGroup buttonsGroup;

    public float fadeDuration = 1f;

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
        // Show background immediately
        backgroundGroup.alpha = 1f;

        buttonsGroup.alpha = 0f;
        SetButtonsInteractable(false);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // Lerp from 0 to 1 based on the elapsed time
            buttonsGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            yield return null;
        }
        // Ensure the alpha is exactly 1 at the end
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
