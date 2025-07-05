using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    public static PauseMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 PauseMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnResumeButton()
    {
        GameManager.instance.TogglePause();
    }

    public void OnSaveButton()
    {
        SaveManager.instance.SaveGame(1);
    }

    public void OnLoadButton()
    {
        if (SaveManager.instance.LoadExists(1))
        {
            SaveManager.instance.LoadGame(1);
        }
    }

    public void OnOptionsButton()
    {
        TurnOff(false);
        OptionsMenu.instance.TurnOn(this);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
