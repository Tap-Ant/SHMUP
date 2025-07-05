using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : Menu
{
    public static GameOverMenu instance = null;
    public Text scoreReadout = null;
    public Text hiScoreReadout = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 GameOverMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnContinueButton()
    {
        if (ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[0].score, (int)GameManager.instance.gameSession.hardness) ||
            ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[1].score, (int)GameManager.instance.gameSession.hardness))
        {
            if (GameManager.instance.twoPlayer)
            {
                if (ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[0].score, (int)GameManager.instance.gameSession.hardness))
                {
                    KeypadMenu.instance.playerIndex = 0;
                }
                else
                {
                    KeypadMenu.instance.playerIndex = 1;
                }

                if (ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[0].score, (int)GameManager.instance.gameSession.hardness) &&
                    ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[1].score, (int)GameManager.instance.gameSession.hardness))
                {
                    KeypadMenu.instance.bothPlayers = true;
                }
                else
                {
                    KeypadMenu.instance.bothPlayers = false;
                }

            }
            else
            {
                KeypadMenu.instance.playerIndex = 0;
                KeypadMenu.instance.bothPlayers = false;
            }
            KeypadMenu.instance.TurnOn(null);
            TurnOff(false);
        }
        else
            SceneManager.LoadScene("MainMenuScene");
    }

    public void GameOver()
    {
        TurnOn(null);
        AudioManager.instance.PlayMusic(AudioManager.Tracks.GameOver, false, 0.5f);
        scoreReadout.text = GameManager.instance.playerDatas[0].score.ToString(); // TODO: player 2

        if (ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[0].score, (int)GameManager.instance.gameSession.hardness))
        {
            hiScoreReadout.gameObject.SetActive(true);
        }
        else
        {
            hiScoreReadout.gameObject.SetActive(false);
        }
    }
}
