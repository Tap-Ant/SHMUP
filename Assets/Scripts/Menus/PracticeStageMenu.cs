using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeStageMenu : Menu
{
    public static PracticeStageMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 PracticeStageMenu!");
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
