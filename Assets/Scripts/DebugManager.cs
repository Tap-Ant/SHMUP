using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager: MonoBehaviour
{
    public static DebugManager instance = null;
    public bool displaying = false;
    public GameObject ROOT = null;
    public Toggle invincibleToggle = null;

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 DebugManager!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void ToggleHUD()
    {
        if (!displaying)
        {
            if (!ROOT)
            {
                Debug.LogError("ROOT GameObject not set!");
            }
            else
            {
                ROOT.SetActive(true);
                displaying = true;
                Time.timeScale = 0; // pause
                Cursor.visible = true;
            }
        }
        else
        {
            if (!ROOT)
            {
                Debug.LogError("ROOT GameObject not set!");
            }
            else
            {
                ROOT.SetActive(false);
                displaying = false;
                Time.timeScale = 1; // resume
                Cursor.visible = false;
            }
        }
    }

    public void ToggleInvincibility()
    {
        if (invincibleToggle)
        {
            GameManager.instance.gameSession.invincible = invincibleToggle.isOn;
        }
    }
}
