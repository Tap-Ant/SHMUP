using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;

    public InputState[] playerState = new InputState[2];
    public InputState[] playerPrevState = new InputState[2];
    public ButtonMapping[] playerButtons = new ButtonMapping[2];
    public AxisMapping[] playerAxis = new AxisMapping[2];
    public KeyButtonMapping[] playerKeyButtons = new KeyButtonMapping[2];
    public KeyAxisMapping[] playerKeyAxis = new KeyAxisMapping[2];

    public int[] playerController = new int[2];
    public bool[] playerUsingKeys = new bool[2];
    public const float deadZone = 0.01f;
    private System.Array allKeyCodes = System.Enum.GetValues(typeof(KeyCode));

    private string[,] playerButtonNames = {{ "J1_B1","J1_B2","J1_B3","J1_B4", "J1_B5", "J1_B6", "J1_B7", "J1_B8" },
                                           { "J2_B1","J2_B2","J2_B3","J2_B4", "J2_B5", "J2_B6", "J2_B7", "J2_B8" }};
    private string[,] playerAxisNames   = {{ "J1_Horizontal","J1_Vertical" },
                                           { "J2_Horizontal","J2_Vertical" }};
    public string[] oldJoysticks = null;
    public static string[] actionNames = {"Shoot","Bomb","Option","Auto","Beam","Menu","Extra2","Extra3" };
    public static string[] axisNames = {"Left","Right","Up","Down" };

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 InputManager!");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialization
        playerController[0] = -1;
        playerController[1] = -1;

        playerUsingKeys[0] = false;
        playerUsingKeys[1] = false;

        playerAxis[0] = new AxisMapping();
        playerAxis[1] = new AxisMapping();
        playerButtons[0] = new ButtonMapping();
        playerButtons[1] = new ButtonMapping();
        playerKeyAxis[0] = new KeyAxisMapping(0);
        playerKeyAxis[1] = new KeyAxisMapping(1);
        playerKeyButtons[0] = new KeyButtonMapping(0);
        playerKeyButtons[1] = new KeyButtonMapping(1);

        playerState[0] = new InputState();
        playerState[1] = new InputState();
        playerPrevState[0] = new InputState();
        playerPrevState[1] = new InputState();

        oldJoysticks = Input.GetJoystickNames();
        StartCoroutine(CheckControllers());
    }

    private bool PlayerIsUsingController(int i)
    {
        if (playerController[0] == i) return true;
        if (GameManager.instance.twoPlayer && playerController[1] == i) return true;
        return false;
    }

    IEnumerator CheckControllers()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(1f);

            string[] currentJoysticks = Input.GetJoystickNames();
            for (int i=0; i < currentJoysticks.Length; i++)
            {
                if (i < oldJoysticks.Length)
                {
                    if (currentJoysticks[i] != oldJoysticks[i])
                    {
                        if (string.IsNullOrEmpty(currentJoysticks[i])) // disconnected
                        {
                            Debug.Log("Controller " + i + " has been disconnected.");
                            if (PlayerIsUsingController(i))
                            {
                                ControllerMenu.instance.whichPlayer = i;
                                ControllerMenu.instance.playerText.text = "Player "+ (i+1) +" controller is disconnected!";
                                ControllerMenu.instance.TurnOn(null);
                                // GameManager.instance.PauseGameplay();
                            }

                        }
                        else // connected
                        {
                            Debug.Log("Controller " + i + " is connected using: " + currentJoysticks[i]);
                        }
                    }
                }
                else
                {
                    Debug.Log("New controller connected");
                }
            }
        }
    }

    void UpdatePlayerState(int playerIndex)
    {
        playerPrevState[playerIndex].shoot   = playerState[playerIndex].shoot;
        playerPrevState[playerIndex].bomb    = playerState[playerIndex].bomb;
        playerPrevState[playerIndex].options = playerState[playerIndex].options;
        playerPrevState[playerIndex].beam    = playerState[playerIndex].beam;

        playerPrevState[playerIndex].left = playerState[playerIndex].left;
        playerPrevState[playerIndex].right = playerState[playerIndex].right;
        playerPrevState[playerIndex].up = playerState[playerIndex].up;
        playerPrevState[playerIndex].down = playerState[playerIndex].down;

        playerState[playerIndex].left = false;
        playerState[playerIndex].right = false;
        playerState[playerIndex].down = false;
        playerState[playerIndex].up = false;

        playerState[playerIndex].shoot = false;
        playerState[playerIndex].bomb = false;
        playerState[playerIndex].options = false;
        playerState[playerIndex].auto = false;
        playerState[playerIndex].beam = false;
        playerState[playerIndex].extra1 = false;
        playerState[playerIndex].extra2 = false;
        playerState[playerIndex].extra3 = false;

        if (Input.GetKey(playerKeyAxis[playerIndex].left)) playerState[playerIndex].left = true;
        if (Input.GetKey(playerKeyAxis[playerIndex].right)) playerState[playerIndex].right = true;
        if (Input.GetKey(playerKeyAxis[playerIndex].up)) playerState[playerIndex].up = true;
        if (Input.GetKey(playerKeyAxis[playerIndex].down)) playerState[playerIndex].down = true;

        if (Input.GetKey(playerKeyButtons[playerIndex].shoot)) playerState[playerIndex].shoot = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].bomb)) playerState[playerIndex].bomb = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].option)) playerState[playerIndex].options = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].auto)) playerState[playerIndex].auto = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].beam)) playerState[playerIndex].beam = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].menu)) playerState[playerIndex].extra1 = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].extra2)) playerState[playerIndex].extra2 = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].extra3)) playerState[playerIndex].extra3 = true;

        if (playerController[playerIndex] < 0)
        {
            UpdateMovement(playerIndex);
            return;
        }

        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].horizontal]) < deadZone) playerState[playerIndex].left = true;
        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].horizontal]) > deadZone) playerState[playerIndex].right = true;
        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].vertical]) < deadZone) playerState[playerIndex].down = true;
        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].vertical]) > deadZone) playerState[playerIndex].up = true;

        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].shoot])) playerState[playerIndex].shoot = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].bomb])) playerState[playerIndex].bomb = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].option])) playerState[playerIndex].options = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].auto])) playerState[playerIndex].auto = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].beam])) playerState[playerIndex].beam = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].menu])) playerState[playerIndex].extra1 = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].extra2])) playerState[playerIndex].extra2 = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].extra3])) playerState[playerIndex].extra3 = true;

        UpdateMovement(playerIndex);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance != null)
        {
            UpdatePlayerState(0);
            UpdatePlayerState(1);
        }
    }

    /* return controller index */
    public int DetectControllerButtonPress()
    {
        int result = -1;

        for (int j=0; j<2; j++)
        {
            for (int b=0; b<8; b++)
            {
                if (Input.GetButton(playerButtonNames[j, b])) return j;
            }
        }

        return result;
    }

    /* return button index */
    public int DetectButtonPress()
    {
        int result = -1;

        for (int j = 0; j < 2; j++)
        {
            for (int b = 0; b < 8; b++)
            {
                if (Input.GetButton(playerButtonNames[j, b])) return b;
            }
        }

        return result;
    }

    public int DetectKeyPress()
    {
        foreach (KeyCode key in allKeyCodes)
        {
            if (Input.GetKey(key)) return ((int)key);
        }
        return -1;
    }

    public bool CheckForPlayerInput(int playerIndex)
    {
        int controller = DetectControllerButtonPress();
        if (controller > -1)
        {
            playerController[playerIndex] = controller;
            playerUsingKeys[playerIndex] = false;
            Debug.Log("Player " + playerIndex + " is set to controller " + controller);
            return true;
        }
        if (DetectKeyPress() > -1)
        {
            playerController[playerIndex] = -1;
            playerUsingKeys[playerIndex] = true;
            Debug.Log("Player " + playerIndex + " is set to keyboard");
            return true;
        }

        return false;
    }

    public string GetButtonName(int playerIndex, int actionID)
    {
        string buttonName = "";
        switch (actionID)
        {
            case 0:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].shoot];
                break;
            case 1:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].bomb];
                break;
            case 2:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].option];
                break;
            case 3:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].auto];
                break;
            case 4:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].beam];
                break;
            case 5:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].menu];
                break;
            case 6:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].extra2];
                break;
            case 7:
                buttonName = playerButtonNames[playerIndex, playerButtons[playerIndex].extra3];
                break;
        }

        char b = buttonName[4];

        return "Button " + b.ToString();
    }

    public string GetKeyName(int playerIndex, int actionID)
    {
        KeyCode key = KeyCode.None;
        switch (actionID)
        {
            case 0:
                key = playerKeyButtons[playerIndex].shoot;
                break;
            case 1:
                key = playerKeyButtons[playerIndex].bomb;
                break;
            case 2:
                key = playerKeyButtons[playerIndex].option;
                break;
            case 3:
                key = playerKeyButtons[playerIndex].auto;
                break;
            case 4:
                key = playerKeyButtons[playerIndex].beam;
                break;
            case 5:
                key = playerKeyButtons[playerIndex].menu;
                break;
            case 6:
                key = playerKeyButtons[playerIndex].extra2;
                break;
            case 7:
                key = playerKeyButtons[playerIndex].extra3;
                break;
        }

        return key.ToString();
    }

    public string GetKeyAxisName(int playerIndex, int actionID)
    {
        KeyCode key = KeyCode.None;
        switch (actionID)
        {
            case 0:
                key = playerKeyAxis[playerIndex].left;
                break;
            case 1:
                key = playerKeyAxis[playerIndex].right;
                break;
            case 2:
                key = playerKeyAxis[playerIndex].up;
                break;
            case 3:
                key = playerKeyAxis[playerIndex].down;
                break;
        }

        return key.ToString();
    }

    public void BindPlayerKey(int player, int actionID, KeyCode key)
    {
        switch (actionID)
        {
            case 0:
                playerKeyButtons[player].shoot = key;
                break;
            case 1:
                playerKeyButtons[player].bomb = key;
                break;
            case 2:
                playerKeyButtons[player].option = key;
                break;
            case 3:
                playerKeyButtons[player].auto = key;
                break;
            case 4:
                playerKeyButtons[player].beam = key;
                break;
            case 5:
                playerKeyButtons[player].menu = key;
                break;
            case 6:
                playerKeyButtons[player].extra2 = key;
                break;
            case 7:
                playerKeyButtons[player].extra3 = key;
                break;
        }
    }

    public void BindPlayerAxisKey(int player, int actionID, KeyCode key)
    {
        switch (actionID)
        {
            case 0:
                playerKeyAxis[player].left = key;
                break;
            case 1:
                playerKeyAxis[player].right = key;
                break;
            case 2:
                playerKeyAxis[player].up = key;
                break;
            case 3:
                playerKeyAxis[player].down = key;
                break;
        }
    }

    public void BindPlayerButton(int player, int actionID, byte button)
    {
        switch (actionID)
        {
            case 0:
                playerButtons[player].shoot = button;
                break;
            case 1:
                playerButtons[player].bomb = button;
                break;
            case 2:
                playerButtons[player].option = button;
                break;
            case 3:
                playerButtons[player].auto = button;
                break;
            case 4:
                playerButtons[player].beam = button;
                break;
            case 5:
                playerButtons[player].menu = button;
                break;
            case 6:
                playerButtons[player].extra2 = button;
                break;
            case 7:
                playerButtons[player].extra3 = button;
                break;
        }
    }

    void UpdateMovement(int playerIndex)
    {
        playerState[playerIndex].movement.x = 0;
        playerState[playerIndex].movement.y = 0;

        if (playerState[playerIndex].right)
        {
            playerState[playerIndex].movement.x += 1;
        }
        if (playerState[playerIndex].left)
        {
            playerState[playerIndex].movement.x += -1;
        }

        if (playerState[playerIndex].up)
        {
            playerState[playerIndex].movement.y += 1;
        }
        if (playerState[playerIndex].down)
        {
            playerState[playerIndex].movement.y += -1;
        }

        playerState[playerIndex].movement.Normalize();
    }
}

public class InputState
{
    public Vector2 movement;
    public bool left, right, up, down;
    public bool shoot, bomb, options, auto, beam, extra1, extra2, extra3;
}

public class ButtonMapping
{
    public byte shoot   = 0;
    public byte bomb    = 1;
    public byte option = 2;
    public byte auto    = 3;
    public byte beam    = 4;
    public byte menu  = 5;
    public byte extra2  = 6;
    public byte extra3  = 7;
}

public class AxisMapping
{
    public byte horizontal = 0;
    public byte vertical = 1;
}

public class KeyButtonMapping
{
    public KeyCode shoot  = KeyCode.B;
    public KeyCode bomb   = KeyCode.N;
    public KeyCode option = KeyCode.M;
    public KeyCode auto   = KeyCode.Comma;
    public KeyCode beam   = KeyCode.Period;
    public KeyCode menu   = KeyCode.J;
    public KeyCode extra2 = KeyCode.K;
    public KeyCode extra3 = KeyCode.L;

    public KeyButtonMapping(int playerIndex)
    {
        if (playerIndex == 0)
        {
            shoot  = KeyCode.B;
            bomb   = KeyCode.N;
            option = KeyCode.M;
            auto   = KeyCode.Comma;
            beam   = KeyCode.Period;
            menu   = KeyCode.J;
            extra2 = KeyCode.K;
            extra3 = KeyCode.L;
        }
        else
        {
            shoot  = KeyCode.Keypad0;
            bomb   = KeyCode.KeypadPeriod;
            option = KeyCode.KeypadEnter;
            auto   = KeyCode.Comma;
            beam   = KeyCode.KeypadPlus;
            menu   = KeyCode.Escape;
            extra2 = KeyCode.Keypad8;
            extra3 = KeyCode.Keypad9;
        }
    }
}

public class KeyAxisMapping
{
    public KeyCode left  = KeyCode.LeftArrow;
    public KeyCode right = KeyCode.RightArrow;
    public KeyCode up    = KeyCode.UpArrow;
    public KeyCode down  = KeyCode.DownArrow;

    public KeyAxisMapping(int playerIndex)
    {
        if (playerIndex == 0)
        {
            left  = KeyCode.A;
            right = KeyCode.D;
            up    = KeyCode.W;
            down  = KeyCode.S;
        }
        else
        {
            left  = KeyCode.LeftArrow;
            right = KeyCode.RightArrow;
            up    = KeyCode.UpArrow;
            down  = KeyCode.DownArrow;
        }
    }
}