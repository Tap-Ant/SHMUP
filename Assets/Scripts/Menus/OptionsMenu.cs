using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : Menu
{
    public static OptionsMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 OptionsMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnAudioButton()
    {
        TurnOff(false);
        AudioOptionsMenu.instance.TurnOn(this);
    }

    public void OnControlsButton()
    {
        TurnOff(false);
        HUD.instance.HideHUD();
        ControlsOptionsMenu.instance.TurnOn(this);
    }

    public void OnGraphicsButton()
    {
        TurnOff(false);
        GraphicsOptionsMenu.instance.TurnOn(this);
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
