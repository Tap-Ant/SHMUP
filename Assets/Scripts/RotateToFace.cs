using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFace : MonoBehaviour
{
    public bool facePlayer = false;
    public bool faceTarget = false;
    public GameObject target = null;
    private bool active = false;

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    void FixedUpdate()
    {
        if (active)
        {
            if (facePlayer || faceTarget)
            {
                Vector2 targetPosition = Vector2.zero;
                if (facePlayer)
                {
                    if (!GameManager.instance || !GameManager.instance.playerCrafts[0])
                        return;

                    targetPosition = GameManager.instance.playerCrafts[0].transform.position;
                    
                }
                else if (faceTarget)
                {
                    if (target == null)
                        return;
                    
                    targetPosition = target.transform.position;
                }

                Vector2 direction = (Vector2)transform.position - targetPosition;
                direction.Normalize();

                transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            }
        }
    }
}
