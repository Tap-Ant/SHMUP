using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPart : MonoBehaviour
{
    public bool destroyed = false;
    public bool damaged = false;

    bool usingDamagedSprite = false;
    public Sprite damagedSprite = null;
    public Sprite destroyedSprite = null;

    public UnityEvent triggerOnDestroyed;

    public int destroyedByPlayer = 2;

    public void Destroyed(int playerIndex)
    {
        if (destroyed) return;

        destroyedByPlayer = playerIndex;

        triggerOnDestroyed.Invoke();

        if (destroyedSprite)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
                spriteRenderer.sprite = destroyedSprite;
        }
        destroyed = true;
        Enemy enemy = transform.root.GetComponent<Enemy>();
        if (enemy)
            enemy.PartDestroyed();
    }

    public void Damaged(bool switchToDamagedSprite)
    {
        if (destroyed) return;

        if (switchToDamagedSprite && !usingDamagedSprite)
        {
            if (damagedSprite)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                    spriteRenderer.sprite = damagedSprite;
            }
            usingDamagedSprite = true;
        }
        damaged = true;
    }
}
