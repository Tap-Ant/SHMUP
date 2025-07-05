using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;
    int currentMultiplier = 1;
    public int[,]    scores = new int[8, 4];
    public string[,] names  = new string[8, 4];

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 ScoreManager!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        for (int h = 0; h < 4; h++)
        {
            for (int s = 0; s < 8; s++)
            {
                names[s, h] = "";
                scores[s, h] = 0;
            }
        }

        LoadScores();
    }

    public void AddScore(int score, int hardness, string name)
    {
        for (int s = 0; s < 8; s++)
        {
            if (score > scores[s,hardness])
            {
                ShuffleScoresDown(s, hardness);
                scores[s,hardness] = score;
                names[s,hardness] = name;
                return;
            }
        }
    }

    private void ShuffleScoresDown(int scoreIndex, int hardness)
    {
        for (int s=7; s>scoreIndex; s--)
        {
            scores[s, hardness] = scores[s - 1, hardness];
            names[s, hardness] = names[s - 1, hardness];
        }
    }

    public bool IsTopScore(int score, int hardness)
    {
        if (score > scores[0, hardness])
            return true;
        return false;
    }

    public bool IsHiScore(int score, int hardness)
    {
        for (int s=7; s >= 0; s--)
        {
            if (score > scores[s, hardness])
                return true;
        }
        return false;
    }

    public void SaveScores()
    {
        string savePath = Application.persistentDataPath + "/scrs.dat";
        Debug.Log("savePath=" + savePath);

        try
        {
            FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(fileStream);
            if (writer != null)
            {
                for (int h = 0; h < 4; h++)
                {
                    for (int s = 0; s < 8; s++)
                    {
                        writer.Write(names[s, h]);
                        writer.Write(scores[s, h]);
                    }
                }
                fileStream.Close(); writer.Close();
            }
            else
            {
                Debug.LogError("Failed to create BinaryWriter for " + savePath + "!");
            }
        }
        catch(System.Exception e)
        {
            Debug.LogWarning(e.Message + " Failed to create FileStream for saving "+savePath+"!");
        }
    }

    public void LoadScores()
    {
        string loadPath = Application.persistentDataPath + "/scrs.dat";
        Debug.Log("savePath=" + loadPath);

        try
        {
            FileStream fileStream = new FileStream(loadPath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fileStream);
            if (reader != null)
            {
                for (int h = 0; h < 4; h++)
                {
                    for (int s = 0; s < 8; s++)
                    {
                        names[s, h] = reader.ReadString();
                        scores[s, h] = reader.ReadInt32();
                    }
                }
                fileStream.Close(); reader.Close();
            }
            else
            {
                Debug.LogError("Failed to create BinaryReader for " + loadPath + "!");
            }
        }
        catch(System.Exception e)
        {
            Debug.LogWarning(e.Message + "Failed to create FileStream for loading " + loadPath + "!");
        }
    }

    public void ShootableHit(int playerIndex, int score)
    {
        GameManager.instance.playerCrafts[playerIndex].IncreaseScore(score * currentMultiplier);
    }

    public void ShootableDestroyed(int playerIndex, int score)
    {
        GameManager.instance.playerCrafts[playerIndex].IncreaseScore(score * currentMultiplier);
    }

    public void BossDestroyed(int playerIndex, int score)
    {
        GameManager.instance.playerCrafts[playerIndex].IncreaseScore(score * currentMultiplier);
    }

    public void PickupCollected(int playerIndex, int score)
    {
        GameManager.instance.playerCrafts[playerIndex].IncreaseScore(score * currentMultiplier);
    }

    public void BulletDestroyed(int playerIndex, int score)
    {
        GameManager.instance.playerCrafts[playerIndex].IncreaseScore(score * currentMultiplier);
    }

    public void MedalCollected(int playerIndex, int score)
    {
        GameManager.instance.playerCrafts[playerIndex].IncreaseScore(score * currentMultiplier);
    }

    public void UpdateChainMultiplier(int playerIndex)
    {
        int chain = GameManager.instance.playerDatas[playerIndex].chain;
        currentMultiplier = (int)Math.Pow((chain + 1), 1.5);
    }

    public int TopScore(int hardness)
    {
        return scores[0, hardness];
    }
}
