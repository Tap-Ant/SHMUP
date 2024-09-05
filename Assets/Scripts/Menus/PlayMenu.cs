using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenu : Menu
{
    public static PlayMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 PlayMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnDifficultyButton()
    {
        TurnOff(false);
        CraftSelectMenu.instance.TurnOn(this);
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
