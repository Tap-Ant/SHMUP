using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    public EnemyRule[] rules;
    private EnemyPattern pattern;
    private EnemySection[] sections;
    public bool isBoss = false;

    private int timer;
    public int timeout = 3600;
    private bool timedOut = false;

    Animator animator = null;
    public string timeoutParameterName = null;

    private WaveTrigger owningWave = null;

    private void Start()
    {
        sections = gameObject.GetComponentsInChildren<EnemySection>();
        animator = gameObject.GetComponentInChildren<Animator>();
        timer = timeout;
    }

    public void SetWave(WaveTrigger wave)
    {
        owningWave = wave;
    }

    public void SetPattern(EnemyPattern inPattern)
    {
        pattern = inPattern;
    }

    private void FixedUpdate()
    {
        // timeout
        if (isBoss)
        {
            if (timer <= 0 && !timedOut)
            {
                timedOut = true;
                if (animator)
                    animator.SetTrigger(timeoutParameterName);
                sections[0].EnableState("TimeOut");
            }
            else timer--;
        }
        else
        {
            data.progressTimer++;
            if (pattern)
                pattern.Calculate(transform, data.progressTimer);
        }

        // off screen check
        float y = transform.position.y;
        if (GameManager.instance && GameManager.instance.progressWindow)
            y -= GameManager.instance.progressWindow.data.positionY;
        if (y < -200)
            OutOfBounds();

        // Update state timer
        foreach (EnemySection section in sections)
            section.UpdateStateTimers();
    }

    public void TimeOutDestruct()
    {
        Destroy(gameObject);
    }

    void OutOfBounds()
    {
        Destroy(gameObject);
    }

    public void EnableState(string name)
    {
        foreach(EnemySection section in sections)
        {
            section.EnableState(name);
        }
    }

    public void DisableState(string name)
    {
        foreach (EnemySection section in sections)
        {
            section.DisableState(name);
        }
    }

    public void PartDestroyed()
    {
        // Go through all rules and check for parts matching ruleset
        foreach (EnemyRule rule in rules)
        {
            if (!rule.triggered)
            {
                int noOfDestroyedParts = 0;
                foreach (EnemyPart part in rule.partsToCheck)
                {
                    if (part.destroyed)
                        noOfDestroyedParts++;
                }
                if (noOfDestroyedParts >= rule.noOfPartsRequired)
                {
                    rule.triggered = true;
                    rule.ruleEvents.Invoke();
                }
            }
        }
    }

    public void Destroyed(int triggeredFromRuleIndex)
    {
        EnemyRule triggeredRule = rules[triggeredFromRuleIndex];
        int playerIndex = triggeredRule.partsToCheck[0].destroyedByPlayer; // todo Check if index 0 is safe
        if (owningWave)
        {
            owningWave.EnemyDestroyed(transform.position, playerIndex);
        }
        Destroy(gameObject);
    }
}

[Serializable]
public struct EnemyData
{
    public float progressTimer;

    public float positionX;
    public float positionY;

    public int patternUID;
}
