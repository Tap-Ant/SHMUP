using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptionsMenu : Menu
{
    public static GraphicsOptionsMenu instance = null;
    public Toggle fullScreenToggle = null;
    public Button nextButton = null;
    public Button prevButton = null;
    public Text resolutionText = null;

    public bool fullScreenToApply = true;
    private Resolution resolutionToApply;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 GraphicsOptionsMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (fullScreenToggle)
        {
            fullScreenToggle.isOn = ScreenManager.instance.fullScreen;
        }
        fullScreenToApply = ScreenManager.instance.fullScreen;
        resolutionToApply = ScreenManager.instance.currentResolution;

        if (resolutionText)
        {
            resolutionText.text = resolutionToApply.width + "x" + resolutionToApply.height + " - " + resolutionToApply.refreshRate;
        }
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }

    public void OnApplyButton()
    {
        ScreenManager.instance.fullScreen = fullScreenToApply;
        Screen.fullScreen = fullScreenToApply;

        if (fullScreenToApply)
        {
            Debug.Log("Going Fullscreen");
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            Debug.Log("Going Windowed");
            PlayerPrefs.SetInt("FullScreen", 0);
        }
        PlayerPrefs.Save();
    }

    public void OnFullscreenToggle()
    {
        fullScreenToApply = !fullScreenToApply;
    }

    public void OnNextButton()
    {
        resolutionToApply = ScreenManager.instance.NextResolution(resolutionToApply);
        if (resolutionText)
        {
            resolutionText.text = resolutionToApply.width + "x" + resolutionToApply.height + " - " + resolutionToApply.refreshRate;
        }
    }

    public void OnPrevButton()
    {
        resolutionToApply = ScreenManager.instance.PrevResolution(resolutionToApply);
        if (resolutionText)
        {
            resolutionText.text = resolutionToApply.width + "x" + resolutionToApply.height + " - " + resolutionToApply.refreshRate;
        }
    }
}
