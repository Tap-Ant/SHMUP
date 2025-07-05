using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public enum GameMode
    {
        INVALID,
        Menus,
        Gameplay
    }
    public GameMode gameMode;
    public GameObject gameManagerPrefab = null;
    public int stageNumber = 0;
    private bool initialized = false;
    private Scene displayScene;
    public AudioManager.Tracks playMusicTrack = AudioManager.Tracks.None;

    void Start()
    {
        if (GameManager.instance == null)
        {
            if (gameManagerPrefab)
            {
                Debug.Log("Instantiating gameManager");
                Instantiate(gameManagerPrefab);
                displayScene = SceneManager.GetSceneByName("DisplayScene");
            }
            else
            {
                Debug.LogError("gameManagerPrefab isn't set!");
            }
        }
    }
    void Update()
    {
        if(!initialized)
        {
            if (gameMode == GameMode.INVALID)
                return;
            if (!displayScene.isLoaded)
            {
                SceneManager.LoadScene("DisplayScene", LoadSceneMode.Additive);
            }

            switch (gameMode)
            {
                case GameMode.Menus:
                    MenuManager.instance.SwitchToMainMenuMenus();
                    GameManager.instance.gameState = GameManager.GameState.InMenus;
                    break;
                case GameMode.Gameplay:
                    MenuManager.instance.SwitchToGamePlayMenus();
                    GameManager.instance.gameState = GameManager.GameState.Playing;
                    GameManager.instance.gameSession.stage = stageNumber;
                    break;
            };

            if (playMusicTrack != AudioManager.Tracks.None)
            {
                AudioManager.instance.PlayMusic(playMusicTrack, true, 1);
            }

            if (gameMode == GameMode.Gameplay)
            {
                SaveManager.instance.SaveGame(0); // 0: autosave at stage start
                GameManager.instance.SpawnPlayers();
            }

            initialized = true;
        }
    }
}
