using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable]
public class EnemyStep
{
    public enum MovementType
    {
        INVALID,

        none,      // waiting at a position
        direction,
        spline,    // curvy line
        atTarget,
        homing,
        follow,
        circle,

        NOOFMOVEMENTTYPES
    }

    public enum RotationType
    {
        INVALID,

        none,
        setAngle,
        lookAhead,
        spinning,
        facePlayer,
        
        NOOFROTATIONS
    }

    [SerializeField]
    public MovementType movType;

    [SerializeField]
    public Vector2 direction;

    [SerializeField]
    public Spline spline;

    [SerializeField]
    public RotationType rotType = RotationType.lookAhead;

    [SerializeField]
    public float endAngle = 0f;

    [SerializeField]
    [Range(0.01f, 4)]
    public float angleSpeed = 1f;

    [SerializeField]
    public float noOfSpins = 1f;

    [SerializeField]
    [Range(0.01f, 20.0f)]
    public float movSpeed = 4f;

    [SerializeField]
    public float framesToWait = 30f;

    public List<String> activateStates = new List<String>();
    public List<String> deactivateStates = new List<String>();

    public EnemyStep(MovementType inMovement)
    {
        movType = inMovement;
        direction = Vector2.zero;

        if (inMovement == MovementType.spline)
        {
            spline = new Spline();
        }
    }

    public float TimeToComplete()
    {
        if (movType == MovementType.direction)
        {
            float timeToTravel = direction.magnitude / movSpeed;
            return timeToTravel;
        }
        else if (movType == MovementType.none)
        {
            return framesToWait;
        }
        else if (movType == MovementType.spline)
        {
            return spline.Length() / movSpeed;
        }
        else if (movType == MovementType.homing)
        {
            return framesToWait;
        }

        Debug.LogError("TimeToComplete unprocessed movement type, returning 1");
        return 1;
    }

    public Vector2 EndPosition(Vector3 startPosition)
    {
        Vector2 result = startPosition;
        if (movType == MovementType.direction)
        {
            result += direction;
            return result;
        }
        else if (movType == MovementType.none)
        {
            return startPosition;
        }
        else if (movType == MovementType.spline)
        {
            result += (spline.LastPoint() - spline.StartPoint());
            return result;
        }
        else if (movType == MovementType.homing)
        {
            if (GameManager.instance && GameManager.instance.playerCrafts[0])
                return GameManager.instance.playerCrafts[0].transform.position;
            else
                return startPosition;
        }

        Debug.LogError("EndPosition unprocessed movement type, returning start");
        return result;
    }

    public Vector3 CalculatePosition(Vector2    startPos, 
                                     float      stepTime,
                                     Vector2    oldPosition,
                                     Quaternion oldAngle)
    {
        float normalizedTime = stepTime / TimeToComplete();
        if (normalizedTime < 0) normalizedTime = 0;

        if (movType == MovementType.direction)
        {
            float timeToTravel = direction.magnitude / movSpeed;
            float ratio = 0;
            if (timeToTravel != 0)
                ratio = stepTime / timeToTravel;

            Vector2 place = startPos + (direction * ratio);
            return place;
        }
        else if (movType == MovementType.none)
        {
            return startPos;
        }
        else if (movType == MovementType.spline)
        {
            return spline.GetPosition(normalizedTime) + startPos;
        }
        else if (movType == MovementType.homing) // homing in on player
        {
            Vector2 dir = (oldAngle * Vector2.down);
            Vector2 mov = (dir * movSpeed);
            Vector2 pos = oldPosition + mov;
            return pos;
        }

        Debug.LogError("CalculatePosition unprocessed movement type, returning startPos");
        return startPos;
    }

    public void FireActivateStates(Enemy enemy)
    {
        foreach(string state in activateStates)
        {
            enemy.EnableState(state);
        }
    }

    public void FireDeactivateStates(Enemy enemy)
    {
        foreach (string state in deactivateStates)
        {
            enemy.DisableState(state);
        }
    }

    // TODO This is unfinished
    public float EndRotation()
    {
        return endAngle;
    }

    public Quaternion CalculateRotation(float startRotation, Vector3 currentPosition, Vector3 oldPosition, float time)
    {
        float normalizedTime = time / TimeToComplete();
        if (normalizedTime <= 0)
            normalizedTime = 0;

        if (rotType == RotationType.setAngle)
        {
            Quaternion result = Quaternion.Euler(0, 0, endAngle);
            return result;
        }
        else if (rotType == RotationType.spinning)
        {
            float start = endAngle - (noOfSpins * 360);
            float angle = Mathf.Lerp(start, endAngle, normalizedTime);
            Quaternion result = Quaternion.Euler(0,0,angle);
            return result;
        }
        else if (rotType == RotationType.facePlayer)
        {
            float angle = 0;
            Transform target = null;
            if (GameManager.instance && GameManager.instance.playerCrafts[0])
                target = GameManager.instance.playerCrafts[0].transform;

            if (target)
            {
                Vector2 currentDir = (currentPosition - oldPosition).normalized;
                Vector2 targetDir = (target.transform.position - currentPosition).normalized;
                Vector2 newDir = Vector2.Lerp(currentDir, targetDir, angleSpeed);
                angle = Vector2.SignedAngle(Vector2.down, newDir);
            }


            return Quaternion.Euler(0, 0, angle);
        }
        else if (rotType == RotationType.lookAhead)
        {
            Vector2 dir = (currentPosition - oldPosition).normalized;
            float angle = Vector2.SignedAngle(Vector2.down, dir);
            return Quaternion.Euler(0,0,angle);
        }

        return Quaternion.Euler(0, 0, 0);
    }
}
