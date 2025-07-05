using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeypadMenu : Menu
{
    public static KeypadMenu instance = null;
    public Text enterText = null;
    public Text nameText = null;
    public int playerIndex = 0;
    public bool bothPlayers = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying tor create more than one KeypadMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);
        enterText.text = "Enter name for player " + (playerIndex + 1);
    }

    public void OnEnterButton()
    {
        ScoreManager.instance.AddScore(GameManager.instance.playerDatas[playerIndex].score,
                                       (int)GameManager.instance.gameSession.hardness,
                                       nameText.text);
        ScoreManager.instance.SaveScores();

        if (bothPlayers && playerIndex==0)
        {
            playerIndex = 1;
            enterText.text = "Enter name for player " + (playerIndex + 1);
            nameText.text = "";
        }
        else
        {
            TurnOff(false);
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void OnKeyPress(int key)
    {
        nameText.text += (char)key;
    }

    public void OnClearButton()
    {
        nameText.text = "";
    }

    public void OnDeleteButton()
    {
        if (nameText.text.Length > 0)
            nameText.text = nameText.text.Substring(0, nameText.text.Length - 1);
    }
}
