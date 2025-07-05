using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsMenu : Menu
{
    public static AudioOptionsMenu instance = null;
    public Slider masterVolSlider = null;
    public Slider fxVolSlider = null;
    public Slider musicVolSlider = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 AudioOptionsMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        float volume = 1;
        if (PlayerPrefs.HasKey("MasterVolume"))
            volume = PlayerPrefs.GetFloat("MasterVolume");
        masterVolSlider.value = volume;

        volume = 1;
        if (PlayerPrefs.HasKey("EffectsVolume"))
            volume = PlayerPrefs.GetFloat("EffectsVolume");
        fxVolSlider.value = volume;

        volume = 1;
        if (PlayerPrefs.HasKey("MusicVolume"))
            volume = PlayerPrefs.GetFloat("MusicVolume");
        musicVolSlider.value = volume;
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }

    public void UpdateMasterVolume(float value)
    {
        float volume = Mathf.Clamp(value, 0.0001f, 1);
        AudioManager.instance.mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void UpdateSFXVolume(float value)
    {
        float volume = Mathf.Clamp(value, 0.0001f, 1);
        AudioManager.instance.mixer.SetFloat("EffectsVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("EffectsVolume", volume);
        PlayerPrefs.Save();
    }

    public void UpdateMusicVolume(float value)
    {
        float volume = Mathf.Clamp(value, 0.0001f, 1);
        AudioManager.instance.mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}
