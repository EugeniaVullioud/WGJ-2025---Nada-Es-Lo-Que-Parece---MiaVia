using UnityEngine;

[CreateAssetMenu(menuName = "Game/Consequences/Audio Consecuence")]
public class AudioConsecuence : ConsequenceData
{
    public AudioClip sound;
    public float volume = 1f;
    public bool loop = false;
    public override void Execute()
    {
        EventManager.Instance.OnPlaySFX(sound, volume, loop);
    }
}
