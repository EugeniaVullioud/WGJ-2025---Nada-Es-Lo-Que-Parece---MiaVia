using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer and parameters")]
    public AudioMixer mixer;

    // Names of your exposed parameters in AudioMixer
    public string volumeParam1 = "MasterVolume";
    public string volumeParam2 = "MusicVolume";
    public string volumeParam3 = "SFXVolume";


    [Header("Default Volume Values (0 to 1)")]
    [Range(0f, 1f)] public float defaultMasterVolume = 1f;
    [Range(0f, 1f)] public float defaultMusicVolume = 1f;
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;

    private GenericPool<AudioSource> audioSourcePool;

    [Header("AudioSource prefab for pooling")]
    public AudioSource audioSourcePrefab;

    [Header("Music Control")]
    public AudioSource musicSource;
    public float musicFadeDuration = 1f;



    private void OnEnable()
    {
        EventBus.Subscribe<SetMasterVolume>((e) => SetVolumeParameters(volumeParam1, e.Volume));
        EventBus.Subscribe<SetMusicVolume>((e) => SetVolumeParameters(volumeParam2, e.Volume));
        EventBus.Subscribe<SetSFXVolume>((e) => SetVolumeParameters(volumeParam3, e.Volume));
        EventBus.Subscribe<PlaySFX>((ev) => PlaySound(ev.SFX, ev.Volume, ev.Loop));
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<PlaySFX>((ev) => PlaySound(ev.SFX, ev.Volume, ev.Loop));

    }
    void Awake()
    {
        audioSourcePool = new ComponentPool<AudioSource>(audioSourcePrefab, 10, transform);

        SetVolumeParameters(volumeParam1, defaultMasterVolume);
        SetVolumeParameters(volumeParam2, defaultMusicVolume);
        SetVolumeParameters(volumeParam3, defaultSFXVolume);
    }

    public void SetVolumeParameters(string pipe, float value)
    {
        mixer.SetFloat(pipe, LinearToDecibel(value));
    }

    private float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f)
            return -80f; // effectively silence
        return Mathf.Log10(linear) * 20f;
    }

    public void PlaySound(AudioClip clip, float volume = 1f, bool loop = false)
    {
        AudioSource source = audioSourcePool.Get();
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.Play();

        if (!loop)
        {
            // Return to pool after clip finished playing
            StartCoroutine(ReturnToPoolAfterPlaying(source));
        }
    }

    private System.Collections.IEnumerator ReturnToPoolAfterPlaying(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        audioSourcePool.Return(source);
    }


    #region Music Control

    public void PlayMusic(AudioClip newClip, bool fade = true)
    {
        if (newClip == null)
        {
            Debug.LogWarning("Trying to play a null music clip.");
            return;
        }

        if (musicSource.clip == newClip)
            return; // already playing this clip

        if (fade)
            StartCoroutine(FadeMusicRoutine(newClip));
        else
        {
            musicSource.clip = newClip;
            musicSource.volume = 1f;
            musicSource.Play();
        }
    }

    public void StopMusic(bool fade = true)
    {
        if (fade)
            StartCoroutine(FadeOutMusicRoutine());
        else
            musicSource.Stop();
    }

    private IEnumerator FadeMusicRoutine(AudioClip newClip)
    {
        float timer = 0f;
        float startVolume = musicSource.volume;

        // Fade out current music
        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / musicFadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new music
        timer = 0f;
        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, timer / musicFadeDuration);
            yield return null;
        }

        musicSource.volume = 1f;
    }

    private IEnumerator FadeOutMusicRoutine()
    {
        float timer = 0f;
        float startVolume = musicSource.volume;

        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / musicFadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = 1f;
    }

    #endregion
}
