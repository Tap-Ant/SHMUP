using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public EnemyPattern[] patterns = null;
    public float[] delays = null;

    public int waveBonus = 0;
    public bool spawnCyclicPickup = false;
    public PickUp[] spawnSpecificPickup;

    public int noOfEnemies = 0;

    private void Start()
    {
        foreach (EnemyPattern pattern in patterns)
        {
            if (pattern != null)
                noOfEnemies++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        int i = 0;
        foreach (EnemyPattern pattern in patterns)
        {
            if (pattern)
            {
                pattern.owningWave = this;
                Session.Hardness hardness = GameManager.instance.gameSession.hardness;
                if (delays != null && i < delays.Length) yield return new WaitForSeconds(delays[i]);

                if (pattern.spawnOnEasy && hardness == Session.Hardness.Easy)
                    pattern.Spawn();
                if (pattern.spawnOnNormal && hardness == Session.Hardness.Normal)
                    pattern.Spawn();
                if (pattern.spawnOnHard && hardness == Session.Hardness.Hard)
                    pattern.Spawn();
                if (pattern.spawnOnInsane && hardness == Session.Hardness.Insane)
                    pattern.Spawn();
            }
            i++;
        }
        yield return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach(EnemyPattern pattern in patterns)
        {
            Handles.DrawLine(transform.position, pattern.transform.position);
        }
    }
#endif

    public void EnemyDestroyed(Vector2 pos, int playerIndex)
    {
        noOfEnemies--;
        if (noOfEnemies == 0)
        {
            ScoreManager.instance.ShootableDestroyed(playerIndex, waveBonus);
            if (spawnCyclicPickup)
            {
                PickUp spawn = GameManager.instance.GetNextDrop();
                GameManager.instance.SpawnPickup(spawn, pos);
            }

            foreach(PickUp pickup in spawnSpecificPickup)
            {
                GameManager.instance.SpawnPickup(pickup, pos);
            }
        }
    }
}
