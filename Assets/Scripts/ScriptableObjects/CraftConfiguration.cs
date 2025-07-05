using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftConfig", menuName = "SHMUP/CraftConfiguration")]
public class CraftConfiguration : ScriptableObject
{
    public static int MAX_SHOT_POWER   = 10;
    public static int MAX_SPEED        = 10;
    public static int MAX_BEAM_POWER   = 10;
    public static int MAX_BOMB_POWER   = 10;
    public static int MAX_OPTION_POWER = 10;

    public float speed;
    public byte  bulletStrength;
    public byte  beamPower;
    public byte  bombPower;
    public byte  optionPower;

    public Sprite craftSprite;

    public ShotConfiguration[] shotLevel = new ShotConfiguration[MAX_SHOT_POWER];
}

[Serializable]
public class ShotConfiguration
{
    public int[] spawnerSizes = new int[5];

}
