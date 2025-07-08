using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftSelectMenu : Menu
{
    public static CraftSelectMenu instance = null;

    public Image player1ShipA = null;
    public Image player1ShipB = null;
    public Image player1ShipC = null;
    public Image player2ShipA = null;
    public Image player2ShipB = null;
    public Image player2ShipC = null;

    public Slider powerSlider1 = null;
    public Slider speedSlider1 = null;
    public Slider beamSlider1 = null;
    public Slider bombSlider1 = null;
    public Slider optionsSlider1 = null;
    public Slider powerSlider2 = null;
    public Slider speedSlider2 = null;
    public Slider beamSlider2 = null;
    public Slider bombSlider2 = null;
    public Slider optionsSlider2 = null;

    public Text countDownText = null;
    public GameObject player2Panel = null;
    public Text player2StartText = null;

    private float lastUnscaledTime = 0;
    private float timer = 5.9f;
    private bool countdown = false;

    private int selectedShip1 = 0;
    private int selectedShip2 = 0;
    public Sprite[] shipSprites = new Sprite[3];
    public Sprite[] shipSpritesSelected = new Sprite[3];
    public Sprite[] shipSpritesDisabled = new Sprite[3];
    public CraftConfiguration[] crafts = new CraftConfiguration[3];

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 CraftSelectMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Reset()
    {
        player2StartText.gameObject.SetActive(true);
        player2Panel.SetActive(false);
        GameManager.instance.twoPlayer = false;

        countDownText.gameObject.SetActive(false);
        countdown = false;
        timer = 5.9f;
        UpdateShipSelection();
    }

    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);
        Reset();
    }

    private void FixedUpdate()
    {
        if (InputManager.instance.playerState[0].shoot)
            StartCountdown();

        if (InputManager.instance.playerState[1].shoot && ROOT.gameObject.activeInHierarchy)
        {
            player2StartText.gameObject.SetActive(false);
            player2Panel.SetActive(true);
            GameManager.instance.twoPlayer = true;
            HUD.instance.TurnOnP2(true);
            UpdateShipSelection();
            StopCountdown();
        }
        if (!InputManager.instance.playerPrevState[0].left && InputManager.instance.playerState[0].left)
        {
            if (selectedShip1 > 0)
                selectedShip1--;
            UpdateShipSelection();
        }
        if (!InputManager.instance.playerPrevState[0].right && InputManager.instance.playerState[0].right)
        {
            if (selectedShip1 < 2)
                selectedShip1++;
            UpdateShipSelection();
        }
        if (!InputManager.instance.playerPrevState[1].left && InputManager.instance.playerState[1].left)
        {
            if (selectedShip2 > 0)
                selectedShip2--;
            UpdateShipSelection();
        }
        if (!InputManager.instance.playerPrevState[1].right && InputManager.instance.playerState[1].right)
        {
            if (selectedShip2 < 2)
                selectedShip2++;
            UpdateShipSelection();
        }

        if (countdown)
        {
            float dUnscaled = Time.unscaledTime - lastUnscaledTime;
            lastUnscaledTime = Time.unscaledTime;
            timer -= dUnscaled;
            countDownText.text = ((int)timer).ToString();
            if (timer < 1)
                GameManager.instance.StartGame();
        }
    }

    public void UpdateShipSelection()
    {
        player1ShipA.sprite = shipSprites[0];
        player1ShipB.sprite = shipSprites[1];
        player1ShipC.sprite = shipSpritesDisabled[2];

        if (selectedShip1 == 0)
            player1ShipA.sprite = shipSpritesSelected[0];
        else if (selectedShip1 == 1)
            player1ShipB.sprite = shipSpritesSelected[1];
        else if (selectedShip1 == 2)
            player1ShipC.sprite = shipSpritesSelected[2];

        CraftConfiguration config1 = crafts[selectedShip1];
        speedSlider1.value = config1.speed;
        powerSlider1.value = config1.bulletStrength;
        beamSlider1.value = config1.beamPower;
        bombSlider1.value = config1.bombPower;
        optionsSlider1.value = config1.optionPower;

        if (GameManager.instance.twoPlayer)
        {
            player2ShipA.sprite = shipSprites[0];
            player2ShipB.sprite = shipSprites[1];
            player2ShipC.sprite = shipSpritesDisabled[2];

            if (selectedShip2 == 0)
                player2ShipA.sprite = shipSpritesSelected[0];
            else if (selectedShip2 == 1)
                player2ShipB.sprite = shipSpritesSelected[1];
            else if (selectedShip2 == 2)
                player2ShipC.sprite = shipSpritesSelected[2];

            CraftConfiguration config2 = crafts[selectedShip2];
            speedSlider2.value = config2.speed;
            powerSlider2.value = config2.bulletStrength;
            beamSlider2.value = config2.beamPower;
            bombSlider2.value = config2.bombPower;
            optionsSlider2.value = config2.optionPower;
        }
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }

    public void OnPlayButton()
    {
        StartCountdown();
    }

    private void StartCountdown()
    {
        timer = 5.9f;
        lastUnscaledTime = Time.unscaledTime;
        countdown = true;
        countDownText.gameObject.SetActive(true);
    }

    private void StopCountdown()
    {
        countdown = false;
        countDownText.gameObject.SetActive(false);
    }
}
