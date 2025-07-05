using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;
    const int SAVE_VERSION = 1;

    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than 1 SaveManager!");
            Destroy(instance);
            return;
        }
        
        instance = this;
    }

    public void SaveGame(int slot)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryWriter writer    = new BinaryWriter(memStream);

        writer.Write(SAVE_VERSION);
        writer.Write(GameManager.instance.twoPlayer);
        GameManager.instance.gameSession.Save(writer);
        GameManager.instance.playerDatas[0].Save(writer);
        if (GameManager.instance.twoPlayer)
            GameManager.instance.playerDatas[1].Save(writer);

        string savePath = Application.persistentDataPath + "/slot" + slot + ".dat";
        Debug.Log("SaveGame, savePath: " + savePath);

        try
        {
            FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            memStream.WriteTo(fileStream);

            fileStream.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message + "Failed to create FileStream for saving " + savePath + "!");
        }
        writer.Close(); memStream.Close();
    }

    public void LoadGame(int slot)
    {
        string loadPath = Application.persistentDataPath + "/slot" + slot + ".dat";
        MemoryStream memStream = new MemoryStream();
        
        try
        {
            FileStream fileStream = new FileStream(loadPath, FileMode.Open);
            BinaryReader reader = new BinaryReader(memStream);
            fileStream.CopyTo(memStream);
            memStream.Position = 0;

            int version = reader.ReadInt32();
            if (version == SAVE_VERSION)
            {
                GameManager.instance.twoPlayer = reader.ReadBoolean();
                GameManager.instance.gameSession.Load(reader);
                GameManager.instance.playerDatas[0].Load(reader);
                if (GameManager.instance.twoPlayer)
                    GameManager.instance.playerDatas[1].Load(reader);
                reader.Close(); fileStream.Close();

                GameManager.instance.ResumeGameFromLoad();
            }
            else
            {
                Debug.LogError("LoadGame(): Save file version incorrect!");
                reader.Close(); fileStream.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message + "Failed to create FileStream for loading " + loadPath + "!");
        }
        memStream.Close();
    }

    public void CopySaveToSlot(int slot)
    {
        Debug.Assert(slot > 0);

        string loadPath = Application.persistentDataPath + "/slot0.dat";
        string destPath = Application.persistentDataPath + "/slot"+slot+".dat";
        File.Copy(loadPath, destPath);
    }

    public bool LoadExists(int slot)
    {
        string loadPath = Application.persistentDataPath + "/slot" + slot + ".dat";
        if (File.Exists(loadPath))
            return true;
        return false;
    }
}
