using System;
using System.IO;
using UnityEngine;

[Serializable]
public class Session
{
    public enum Hardness
    {
        Easy,
        Normal,
        Hard,
        Insane
    };

    public CraftData[] craftDatas = new CraftData[2];

    public Hardness hardness = Hardness.Normal;
    public int stage = 1;
    public bool practice      = false;
    public bool arenaPractice = false;
    public bool stagePractice = false;

    // Cheats
    public bool infiniteLives     = false;
    public bool infiniteContinues = false;
    public bool infiniteBombs     = false;
    public bool invincible        = false;
    public bool halfSpeed         = false;
    public bool doubleSpeed       = false;

    public Session()
    {
        craftDatas[0] = new CraftData();
        craftDatas[1] = new CraftData();
    }

    public void Save (BinaryWriter writer)
    {
        craftDatas[0].Save(writer);
        if (GameManager.instance.twoPlayer) craftDatas[1].Save(writer);
        writer.Write((Byte)hardness);
        writer.Write(stage);
    }

    public void Load(BinaryReader reader)
    {
        craftDatas[0].Load(reader);
        if (GameManager.instance.twoPlayer) craftDatas[1].Load(reader);
        hardness = (Hardness) reader.ReadByte();
        stage    = reader.ReadInt32();
    }
}
