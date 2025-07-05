using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using MenuCommand = UnityEditor.MenuCommand;
#endif

public class EnemyPattern : MonoBehaviour
{
    public List<EnemyStep> steps = new List<EnemyStep>();
    public Enemy enemyPrefab;
    private Enemy spawnedEnemy;
    private int UID;
    public bool stayOnLast = true;
    private int currentStateIndex = 0;
    private int previousStateIndex = -1;

    public bool startActive = false;

    public bool spawnOnEasy   = true;
    public bool spawnOnNormal = true;
    public bool spawnOnHard   = false;
    public bool spawnOnInsane = false;

    [HideInInspector]
    public Vector3 lastPosition = Vector3.zero;
    [HideInInspector]
    public Vector3 currentPosition = Vector3.zero;
    [HideInInspector]
    public Quaternion lastAngle = Quaternion.identity;

    public WaveTrigger owningWave = null;

#if UNITY_EDITOR
    [MenuItem("GameObject/SHMUP/EnemyPattern", false, 10)]
    static void CreateEnemyPatternObject(MenuCommand menuCommand)
    {
        Helpers helper = (Helpers) Resources.Load("Helper");
        if (helper != null)
        {
            GameObject go = new GameObject("EnemyPattern" + helper.nextFreePatternID);
            EnemyPattern pattern = go.AddComponent<EnemyPattern>();
            pattern.UID = helper.nextFreePatternID;
            helper.nextFreePatternID++;

            // Register creation with undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
        else Debug.LogError("Could not find Helper");
    }
#endif

    public void Start()
    {
        if (startActive)
            Spawn();
    }

    public void Spawn()
    {
        if (spawnedEnemy == null)
        {
            spawnedEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation).GetComponent<Enemy>();
            spawnedEnemy.SetWave(owningWave);
            spawnedEnemy.SetPattern(this);

            lastPosition = spawnedEnemy.transform.position;
            currentPosition = lastPosition;
        }
    }

    public void Calculate(Transform enemyTransform, float progressTimer)
    {
        Vector3    pos = CalculatePosition(progressTimer);
        Quaternion rot = CalculateRotation(progressTimer);

        enemyTransform.position = pos;
        enemyTransform.rotation = rot;

        if (currentStateIndex != previousStateIndex)
        {
            // state has changed
            if (previousStateIndex >= 0)
            {
                // call deactivate state
                EnemyStep prevStep = steps[previousStateIndex];
                prevStep.FireDeactivateStates(spawnedEnemy);
            }
            if (currentStateIndex >= 0)
            {
                // call activate states
                EnemyStep currStep = steps[currentStateIndex];
                currStep.FireActivateStates(spawnedEnemy);
            }
            previousStateIndex = currentStateIndex;
        }
    }

    // Calculates stateIndex
    public Vector2 CalculatePosition(float progressTimer)
    {

        if (currentStateIndex < 0)
        {
            if (spawnedEnemy) return spawnedEnemy.transform.position;
            return Vector2.zero;
        }
        lastPosition = currentPosition;

        EnemyStep step = steps[currentStateIndex];
        float stepTime = progressTimer - StartTime(currentStateIndex);
        Vector3 startPos = EndPosition(currentStateIndex - 1);

        currentPosition = step.CalculatePosition(startPos, stepTime, lastPosition, lastAngle);
        return currentPosition;
    }

    public Quaternion CalculateRotation(float progressTimer)
    {
        currentStateIndex = WhichStep(progressTimer);
        if (currentStateIndex < 0)
            return Quaternion.identity;
        float startRotation = 0;
        if (currentStateIndex > 0)
            startRotation = steps[currentStateIndex - 1].EndRotation();
        float stepTime = progressTimer - StartTime(currentStateIndex);
        lastAngle = steps[currentStateIndex].CalculateRotation(startRotation,
                                                                currentPosition,
                                                                lastPosition,
                                                                stepTime);

        return lastAngle;
    }

    int WhichStep(float timer)
    {
        float timeToCheck = timer;
        for (int s=0; s < steps.Count; s++)
        {
            if (timeToCheck < steps[s].TimeToComplete())
                return s;
            timeToCheck -= steps[s].TimeToComplete();
        }

        if (stayOnLast)
            return steps.Count - 1;
        return -1;
    }

    public float StartTime(int step)
    {
        if (step <= 0) return 0;

        float result = 0;
        for (int s=0; s<step; s++)
        {
            result += steps[s].TimeToComplete();
        }

        return result;
    }

    public Vector3 EndPosition(int stepIndex)
    {
        Vector3 result = transform.position;
        if (stepIndex >= 0)
        {
            for (int s=0; s <= stepIndex; s++)
            {
                result = steps[s].EndPosition(result);
            }
        }
        return result;
    }

    public EnemyStep AddStep(EnemyStep.MovementType movement)
    {
        EnemyStep newStep = new EnemyStep(movement);
        steps.Add(newStep);
        return newStep;
    }

    private void OnValidate()
    {
        foreach (EnemyStep step in steps)
        {
            if (step.movSpeed < 0.5f)
                step.movSpeed = 0.5f;

            if (step.movType == EnemyStep.MovementType.spline)
            {
                step.spline.CalculatePoints(step.movSpeed);
            }
        }
    }

    public float TotalTime()
    {
        float result = 0;
        foreach (EnemyStep step in steps)
        {
            result += step.TimeToComplete();
        }
        return result;
    }
}
