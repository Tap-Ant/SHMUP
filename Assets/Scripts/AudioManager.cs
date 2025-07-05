using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioMixer  mixer        = null;
    public AudioSource musicSource1 = null;
    public AudioSource musicSource2 = null;
    public AudioSource sfxSource    = null;

    public enum Tracks
    {
        Level01,
        //Level02,
        Boss01,
        //Boss02,
        GameOver,
        //Won,
        Menu,
        None,
    }

    public AudioClip[] musicTracks;
    private int activeMusicSource = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 AudioManager!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Restore preferences
        float volume = 1;
        if (PlayerPrefs.HasKey("MasterVolume"))
            volume = PlayerPrefs.GetFloat("MasterVolume");
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        volume = 1;
        if (PlayerPrefs.HasKey("EffectsVolume"))
            volume = PlayerPrefs.GetFloat("EffectsVolume");
        mixer.SetFloat("EffectsVolume", Mathf.Log10(volume) * 20);

        volume = 1;
        if (PlayerPrefs.HasKey("MusicVolume"))
            volume = PlayerPrefs.GetFloat("MusicVolume");
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void PlayMusic(Tracks track, bool fade, float fadeDuration)
    {
        if (activeMusicSource == 0 || activeMusicSource == 2)
        {
            if (!fade)
            {
                mixer.SetFloat("Music1Volume", Mathf.Log10(1)*20);
                mixer.SetFloat("Music2Volume", Mathf.Log10(0.0001f));
                musicSource2.Stop();
            }
            else
            {
                if (activeMusicSource == 0)
                {
                    mixer.SetFloat("Music1Volume", Mathf.Log10(0.0001f));
                    mixer.SetFloat("Music2Volume", Mathf.Log10(0.0001f));
                }
            }
            musicSource1.clip = musicTracks[(int)track];
            StartCoroutine(DelayedPlayMusic(1));
            activeMusicSource = 1;

            if (fade)
            {
                StartCoroutine(Fade(1, fadeDuration, 0, 1));
                if (activeMusicSource == 2)
                    StartCoroutine(Fade(2, fadeDuration, 1, 0));
            }
        }
        else if (activeMusicSource == 1)
        {
            if (!fade)
            {
                mixer.SetFloat("Music1Volume", Mathf.Log10(0.0001f));
                mixer.SetFloat("Music2Volume", Mathf.Log10(1)*20);
                musicSource1.Stop();
            }
            musicSource2.clip = musicTracks[(int)track];
            StartCoroutine(DelayedPlayMusic(2));
            activeMusicSource = 2;

            if (fade)
            {
                StartCoroutine(Fade(2, fadeDuration, 0, 1));
                StartCoroutine(Fade(1, fadeDuration, 1, 0));
            }
        }
    }

    IEnumerator DelayedPlayMusic(int sourceIndex)
    {
        yield return null;
        if (sourceIndex == 1)
            musicSource1.Play();
        else if (sourceIndex == 2)
            musicSource2.Play();
    }

    IEnumerator Fade(int sourceIndex, float duration, float startVolume, float targetVolume)
    {
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float newVol = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            newVol = Mathf.Clamp(newVol, 0.0001f, 0.9999f);

            if (sourceIndex == 1)
                mixer.SetFloat("Music1Volume", Mathf.Log10(newVol) * 20);
            else if (sourceIndex == 2)
                mixer.SetFloat("Music2Volume", Mathf.Log10(newVol) * 20);

            yield return null;
        }

        if (targetVolume <= 0.001f)
        {
            if (sourceIndex == 1)
                musicSource1.Stop();
            else if (sourceIndex == 2)
                musicSource2.Stop();
        }

        yield return null;
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PauseMusic()
    {
        musicSource1.Pause();
        musicSource2.Pause();
    }

    public void ResumeMusic()
    {
        musicSource1.UnPause();
        musicSource2.UnPause();
    }
}
