using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    public ProgressData data;
    public int levelSize;
    public GameObject midGroundTileGrid;
    public float midGroundRate = 0.75f;
    public AnimationCurve speedCurve = new AnimationCurve();

    private Craft player1Craft;
    private Craft player2Craft;
    public bool disableMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        data.positionX = transform.position.x;
        data.positionY = transform.position.y;

        if (GameManager.instance)
        {
            GameManager.instance.progressWindow = this;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.instance) return;
        //CraftData craftData = GameManager.instance.gameSession.craftDatas[0];
        if (data.progress < levelSize)
        {
            if (player1Craft == null)
            {
                player1Craft = GameManager.instance.playerCrafts[0];
            }
            if (player2Craft == null)
            {
                player2Craft = GameManager.instance.playerCrafts[1];
            }
            if (!disableMovement)
            {
                if (player1Craft || player2Craft)
                {
                    float ratio = (float)data.progress / (float)levelSize;
                    float movement = speedCurve.Evaluate(ratio);
                    data.progress++;
                    CraftData craftData = GameManager.instance.gameSession.craftDatas[0];
                    UpdateProgressWindow(craftData.positionX, movement);
                }
            }
        }
    }

    void UpdateProgressWindow(float shipX, float movement)
    {
        data.positionX = shipX/10f;
        data.positionY += movement;
        transform.position = new Vector3(data.positionX, data.positionY, 0);
        midGroundTileGrid.transform.position = new Vector3(0, data.positionY * midGroundRate, 0);
    }
}

[Serializable]
public class ProgressData
{
    public int progress;
    public float positionX;
    public float positionY;
}
