using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public bool twoPlayer = false;
    public GameObject[] craftPrefabs;
    public Craft[] playerCrafts = new Craft[2];
    public PlayerData[] playerDatas;
    public BulletManager bulletManager = null;
    public LevelProgress progressWindow = null;
    public Session gameSession = new Session();

    public PickUp[] cyclicDrops = new PickUp[15];
    public PickUp[] medals = new PickUp[10];
    private int currentDropIndex = 0;
    private int currentMedalIndex = 0;
    public PickUp option = null;
    public PickUp powerup = null;
    public PickUp beamup = null;

    public enum GameState
    {
        INVALID,
        InMenus,
        Playing,
        Paused
    }
    public GameState gameState = GameState.INVALID;

    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 GameManager!");
            Destroy(gameObject);
            return;
        }

        playerDatas = new PlayerData[2];
        playerDatas[0] = new PlayerData();
        playerDatas[1] = new PlayerData();

        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("GameManager created.");

        bulletManager = GetComponent<BulletManager>();

        Application.targetFrameRate = 60;
    }

    public void SpawnPlayer(int playerIndex, int craftType)
    {
        Debug.Assert(craftType < craftPrefabs.Length);
        Debug.Log("Spawning player "+playerIndex);
        if (progressWindow == null)
        {
            progressWindow = GameObject.FindObjectOfType<LevelProgress>();
        }
        Vector3 pos = progressWindow.transform.position;
        int whichPrefab = playerIndex; //TODO: use craftType
        playerCrafts[playerIndex] = Instantiate(craftPrefabs[whichPrefab], pos, Quaternion.identity).GetComponent<Craft>();
        playerCrafts[playerIndex].playerIndex = playerIndex;
        if (twoPlayer)
        {
            if (playerIndex == 0)
                gameSession.craftDatas[playerIndex].positionX = -50;
            else
                gameSession.craftDatas[playerIndex].positionX = 50;
        }
    }

    public void SpawnPlayers()
    {
        SpawnPlayer(0, 0); //TODO: craft type
        if (twoPlayer)
            SpawnPlayer(1, 0);
    }

    public void DelayedRespawn(int playerIndex)
    {
        StartCoroutine(RespawnCoroutine(playerIndex));
    }

    IEnumerator RespawnCoroutine(int playerIndex)
    {   
        yield return new WaitForSeconds(1.5f);
        SpawnPlayer(playerIndex, 0); //TODO: craft type
        yield return null;
    }

    public void ResetState(int playerIndex)
    {
        CraftData craftData = gameSession.craftDatas[playerIndex];
        craftData.positionX = 0;
        craftData.positionY = 0;
        craftData.shotPower = 0;
        craftData.noOfEnabledOptions = 0;
        craftData.optionLayout = 0;
        craftData.beamFiring = false;
        craftData.beamCharge = 0;
        craftData.beamTimer = 0;
        craftData.smallBombs = 0;
        craftData.largeBombs = 0;
    }

    public void RestoreState(int playerIndex)
    {
        int number = gameSession.craftDatas[playerIndex].noOfEnabledOptions;
        gameSession.craftDatas[playerIndex].noOfEnabledOptions = 0;
        gameSession.craftDatas[playerIndex].positionX = 0;
        gameSession.craftDatas[playerIndex].positionY = 0;
        for (int o=0; o<number; o++)
        {
            playerCrafts[playerIndex].AddOption(0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Debug Explode
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (playerCrafts[0])
            {
                playerCrafts[0].Explode();
            }
            if (playerCrafts[1])
            {
                playerCrafts[1].Explode();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DebugManager.instance.ToggleHUD();
        }
    }

    public void StartGame()
    {
        gameState = GameState.Playing;
        ResetState(0);
        ResetState(1);
        playerDatas[0].ResetData();
        playerDatas[1].ResetData();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Stage01");
    }

    public void TogglePause()
    {
        if (gameState == GameState.Playing) // Pause
        {
            gameState = GameState.Paused;
            AudioManager.instance.PauseMusic();
            PauseMenu.instance.TurnOn(null);
            if (DebugManager.instance.displaying)
                DebugManager.instance.ToggleHUD();
            Time.timeScale = 0;
        }
        else // Resume
        {
            gameState = GameState.Playing;
            AudioManager.instance.ResumeMusic();
            PauseMenu.instance.TurnOff(false);
            Time.timeScale = 1;
        }
    }

    public void PickUpFallOffScreen(PickUp pickup)
    {
        if (pickup.config.type == PickUp.PickupType.Medal)
        {
            currentMedalIndex = 0;
        }
    }

    public PickUp GetNextDrop()
    {
        PickUp result = cyclicDrops[currentDropIndex];

        if (result.config.type == PickUp.PickupType.Medal)
        {
            result = medals[currentMedalIndex];
            currentMedalIndex++;
            if (currentMedalIndex > 9)
                currentMedalIndex = 0;
        }

        currentDropIndex++;
        if (currentDropIndex > 14)
            currentDropIndex = 0;

        return result;
    }

    public PickUp SpawnPickup(PickUp pickupPrefab, Vector2 pos)
    {
        PickUp p = Instantiate(pickupPrefab, pos, Quaternion.identity);
        if (p)
            p.transform.SetParent(GameManager.instance.transform);
        return p;
    }

    public void ResumeGameFromLoad()
    {
        gameState = GameState.Playing;
        switch(gameSession.stage)
        {
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Stage01");
                break;
            case 2:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Stage02");
                break;
        }
    }

    public void NextStage()
    {
        HUD.instance.FadeOut();
        if (gameSession.stage == 1)
        {
            gameSession.stage = 2;
            SceneManager.LoadScene("Stage02");
        }
        else if (gameSession.stage == 2)
        {
            VictoryMenu.instance.TurnOn(null);
        }
        HUD.instance.FadeIn();
    }
}
