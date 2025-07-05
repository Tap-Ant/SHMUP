using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PickUpConfig", menuName ="SHMUP/PickUpConfig")]
public class PickUpConfig : ScriptableObject
{
    public PickUp.PickupType type;

    public int   powerLevel   = 1;
    public int   bombPower    = 1;
    public int   medalValue   = 100;
    public int   medalLevel   = 1;
    public int   coinValue    = 1;
    public int   surplusValue = 100;
    public float fallSpeed    = 0;
}
