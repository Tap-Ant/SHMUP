using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance = null;
    public bool fullScreen = true;
    Resolution currentResolution;
    Resolution[] allResolutions;

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
        Screen.SetResolution(res.width, res.height, FullScreenMode.ExclusiveFullScreen, res.refreshRate);
    }

    void RestoreSettings()
    {
        if (!PlayerPrefs.HasKey("FullScreen"))//
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
}
