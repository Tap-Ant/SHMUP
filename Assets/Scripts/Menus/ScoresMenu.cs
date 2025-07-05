using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresMenu : Menu
{
    public static ScoresMenu instance = null;
    public Text[] scores;
    public Text[] names;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 ScoresMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }

    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);

        int hardness = 1;
        for (int s = 0; s < 8; s++)
        {
            scores[s].text = ScoreManager.instance.scores[s,hardness].ToString();
            names[s].text = ScoreManager.instance.names[s,hardness];
        }
    }
}
