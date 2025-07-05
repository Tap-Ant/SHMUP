using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance = null;
    public bool fullScreen = true;
    public Resolution currentResolution;
    public Resolution[] allResolutions;

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than one ScreenManager!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        currentResolution = Screen.currentResolution;
        allResolutions = Screen.resolutions;
    }

    public void SetResolution(Resolution res)
    {
        if (fullScreen)
        {
            Screen.SetResolution(res.width, res.height, FullScreenMode.ExclusiveFullScreen, res.refreshRate);
        }
        else
        {
            Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed, res.refreshRate);
        }
        PlayerPrefs.SetInt("ScreenWidth", res.width);
        PlayerPrefs.SetInt("ScreenHeight", res.height);
        PlayerPrefs.SetInt("ScreenRate", res.refreshRate);

        Cursor.visible = false;
    }

    void RestoreSettings()
    {
        // Restore Resolution settings
        int width = 1280;
        int height = 720;
        int rate = 60;
        if (PlayerPrefs.HasKey("ScreenWidth"))  width  = PlayerPrefs.GetInt("ScreenWidth");
        if (PlayerPrefs.HasKey("ScreenHeight")) height = PlayerPrefs.GetInt("ScreenHeight");
        if (PlayerPrefs.HasKey("ScreenRate"))   rate   = PlayerPrefs.GetInt("ScreenRate");
        Resolution res = FindResolution(width, height, rate);
        SetResolution(res);

        // Restore Fullscreen settings
        if (!PlayerPrefs.HasKey("FullScreen")) //?
        {
            int fullScreenInt = PlayerPrefs.GetInt("FullScreen");
            if (fullScreenInt == 0)
                fullScreen = false;
            else if (fullScreenInt == 1)
                fullScreen = true;
            else
                Debug.LogError("FullScreen preference is invalid!");
        }
        Screen.fullScreen = fullScreen;
    }

    Resolution FindResolution(int width, int height, int rate)
    {
        foreach(Resolution res in allResolutions)
        {
            if (res.width == width && res.height == height && res.refreshRate == rate)
            {
                return res;
            }
        }

        return currentResolution;
    }

    public Resolution NextResolution(Resolution currentResolution)
    {
        int currentIndex = FindResolutionIndex(currentResolution);
        currentIndex++;
        if (currentIndex >= allResolutions.Length)
        {
            currentIndex = 0;
        }
        return allResolutions[currentIndex];
    }

    public Resolution PrevResolution(Resolution currentResolution)
    {
        int currentIndex = FindResolutionIndex(currentResolution);
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = allResolutions.Length - 1;
        }
        return allResolutions[currentIndex];
    }

    int FindResolutionIndex(Resolution currentResolution)
    {
        int index = 0;
        foreach (Resolution res in allResolutions)
        {
            if (currentResolution.width == res.width &&
                currentResolution.height == res.height &&
                currentResolution.refreshRate ==  res.refreshRate)
            {
                return index;
            }
            index++;
        }
        return -1;
    }
}
