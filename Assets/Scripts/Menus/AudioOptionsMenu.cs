using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOptionsMenu : Menu
{
    public static AudioOptionsMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 AudioOptionsMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
