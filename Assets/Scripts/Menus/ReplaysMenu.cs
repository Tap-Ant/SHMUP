using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaysMenu : Menu
{
    public static ReplaysMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 ReplaysMenu!");
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
