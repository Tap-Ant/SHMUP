using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalsMenu : Menu
{
    public static MedalsMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 MedalsMenu!");
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
