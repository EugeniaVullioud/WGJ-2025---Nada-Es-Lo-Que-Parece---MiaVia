using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class EndingMenuController : FadeInMenuBase
{
    [Header("UI Elements")]
    public CanvasGroup buttonsGroup;
    public VideoPlayer videoPlayer;

    private Coroutine videoRoutine;

    [Header("Fade Durations")]
    public float buttonsFadeDuration = 1f;

    protected override (CanvasGroup group, float duration)[] FadeSequence => new[]
    {
        (buttonsGroup, buttonsFadeDuration)
    };

    public override void OnEnableMenu()
    {
        Debug.Log("GANOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
        gameObject.SetActive(true);
        buttonsGroup.gameObject.SetActive(false);
        buttonsGroup.alpha = 0f;

        EventManager.Instance.OnMuteMusic();
        if (videoRoutine != null)  StopCoroutine(videoRoutine);
        videoRoutine = StartCoroutine(PlayVideoAndWaitRoutine());
    }

    public override void OnDisableMenu()
    {
        if (videoRoutine != null)  StopCoroutine(videoRoutine);

        videoPlayer.Stop();
        gameObject.SetActive(false);
    }

    private IEnumerator PlayVideoAndWaitRoutine()
    {
        // Play video
        videoPlayer.Play();
        // Wait for video to finish

        float l = (float)videoPlayer.length;
        yield return new WaitForSeconds(l);

        buttonsGroup.gameObject.SetActive(true);

        // Hide loading text, show start button
        StartCoroutine(FadeOutCanvasGroup(buttonsGroup, 0.5f));

    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup cg, float duration)
    {
        float elapsed = 0f;
        float startAlpha = cg.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
            yield return null;
        }
        cg.alpha = 1f;
        EventManager.Instance.OnUnMuteMusic();

    }
}
