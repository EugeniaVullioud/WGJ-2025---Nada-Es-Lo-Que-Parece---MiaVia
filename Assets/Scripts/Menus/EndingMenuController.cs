using UnityEngine;

public class EndingMenuController : FadeInMenuBase
{
    [Header("UI Elements")]
    public CanvasGroup imagesGroup;
    public CanvasGroup textGroup;
    public CanvasGroup buttonsGroup;

    [Header("Fade Durations")]
    public float imagesFadeDuration = 1f;
    public float textFadeDuration = 1f;
    public float buttonsFadeDuration = 1f;

    protected override (CanvasGroup group, float duration)[] FadeSequence => new[]
    {
        (imagesGroup, imagesFadeDuration),
        (textGroup, textFadeDuration),
        (buttonsGroup, buttonsFadeDuration)
    };
}
