using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int index;
    public byte playerIndex; // player that shot this bullet
}

[Serializable]
public struct BulletData
{
    public float positionX;
    public float positionY;
    public float dX;
    public float dY;
    public float angle;
    public float dAngle;
    public int   type;
    public bool  active;
    public bool  homing;

    public BulletData(float inX,
                      float inY,
                      float inDX,
                      float inDY,
                      float inAngle,
                      float inDangle,
                      int   inType,
                      bool  inActive,
                      bool  inHoming)
    {
        positionX = inX;
        positionY = inY;
        dX        = inDX;
        dY        = inDY;
        angle     = inAngle;
        dAngle    = inDangle;
        type      = inType;
        active    = inActive;
        homing    = inHoming;
    }
}
