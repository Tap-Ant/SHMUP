using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeMenu : Menu
{
    public static PracticeMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 PracticeMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnStageButton()
    {
        TurnOff(false);
        PracticeStageMenu.instance.TurnOn(this);
    }

    public void OnArenaButton()
    {
        TurnOff(false);
        PracticeArenaMenu.instance.TurnOn(this);
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
