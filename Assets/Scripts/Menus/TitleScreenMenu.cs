using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenMenu : Menu
{
    public static TitleScreenMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 TitleScreenMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if (ROOT.gameObject.activeInHierarchy)
        {
            if (InputManager.instance.CheckForPlayerInput(0))
            {
                TurnOff(false);
                MainMenu.instance.TurnOn(this);
            }
        }
    }
}
