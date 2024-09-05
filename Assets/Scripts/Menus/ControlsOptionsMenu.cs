using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsOptionsMenu : Menu
{
    public static ControlsOptionsMenu instance = null;

    public Button[] p1_buttons = new Button[8];
    public Button[] p2_buttons = new Button[8];
    public Button[] p1_keys    = new Button[12];
    public Button[] p2_keys    = new Button[12];

    public GameObject bindingPanel = null;
    public Text       bindText     = null;
    public EventSystem eventSystem = null;

    private bool bindingKey    = false;
    private bool bindingAxis   = false;
    private bool bindingButton = false;
    private int  actionBinding = -1;
    private int  playerBinding = -1;

    private bool waiting = false;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 ControlsOptionsMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnEnable()
    {
        UpdateButtons();
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }

    private void UpdateButtons()
    {
        /* joystick buttons */
        for (int b=0; b<8; b++)
        {
            p1_buttons[b].GetComponentInChildren<Text>().text = InputManager.instance.GetButtonName(0, b);
            p2_buttons[b].GetComponentInChildren<Text>().text = InputManager.instance.GetButtonName(1, b);
        }
        /* key "buttons" */
        for (int k = 0; k < 8; k++)
        {
            p1_keys[k].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyName(0, k);
            p2_keys[k].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyName(1, k);
        }
        /* key "axes" */
        for (int a = 0; a < 4; a++)
        {
            p1_keys[a+8].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyAxisName(0, a);
            p2_keys[a+8].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyAxisName(1, a);
        }
    }

    public void BindP1Key(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 1 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey    = true;
        bindingAxis   = false;
        bindingButton = false;
        playerBinding = 0;
        actionBinding = actionID;

        waiting = true;
    }

    public void BindP1AxisKey(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 1 " + InputManager.axisNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey    = true;
        bindingAxis   = true;
        bindingButton = false;
        playerBinding = 0;
        actionBinding = actionID;

        waiting = true;
    }

    public void BindP1Button(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for Player 1 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey    = false;
        bindingAxis   = false;
        bindingButton = true;
        playerBinding = 0;
        actionBinding = actionID;

        waiting = true;
    }

    public void BindP2Key(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 2 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey    = true;
        bindingAxis   = false;
        bindingButton = false;
        playerBinding = 1;
        actionBinding = actionID;

        waiting = true;
    }

    public void BindP2AxisKey(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 2 " + InputManager.axisNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey    = true;
        bindingAxis   = true;
        bindingButton = false;
        playerBinding = 1;
        actionBinding = actionID;

        waiting = true;
    }

    public void BindP2Button(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for Player 2 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey    = false;
        bindingAxis   = false;
        bindingButton = true;
        playerBinding = 1;
        actionBinding = actionID;

        waiting = true;
    }

    private void Update()
    {
        if (bindingKey || bindingButton)
        {
            if (waiting)
            {
                if (Input.anyKey) return;
                if (InputManager.instance.DetectButtonPress() > -1) return;
                waiting = false;
            }
            else
            {
                if (bindingKey)
                {
                    foreach (KeyCode key in KeyCode.GetValues(typeof(KeyCode)))
                    {
                        if (!key.ToString().Contains("Joystick"))
                        {
                            if (Input.GetKeyDown(key))
                            {
                                if (bindingAxis)
                                {
                                    InputManager.instance.BindPlayerAxisKey(playerBinding, actionBinding, key);
                                }
                                else
                                {
                                    InputManager.instance.BindPlayerKey(playerBinding, actionBinding, key);

                                }
                                bindingPanel.SetActive(false);
                                bindingKey = false;
                                bindingButton = false;
                                eventSystem.gameObject.SetActive(true);
                                UpdateButtons();
                            }
                        }
                    }
                }
                else if (bindingButton)
                {
                    int button = InputManager.instance.DetectButtonPress();
                    if (button > -1)
                    {
                        InputManager.instance.BindPlayerButton(playerBinding, actionBinding, (byte)button);
                        bindingPanel.SetActive(false);
                        bindingKey = false;
                        bindingButton = false;
                        eventSystem.gameObject.SetActive(true);
                        UpdateButtons();
                    }
                }
            }
        }
    }
}
