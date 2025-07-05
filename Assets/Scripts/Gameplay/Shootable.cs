using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public int health = 10;
    public float radiusOrWidth = 10;
    public float height = 10;
    public bool box = false;
    public bool polygon = false;

    public bool remainDestroyed = false;
    private bool destroyed = false;
    public int damageHealth = 5; // at what health is damage sprite displayed

    public int hitScore = 10;
    public int destroyScore = 100;

    private Collider2D polyCollider;
    private int layerMask = 0;

    private Vector2 halfExtent;

    public bool damagedByBullets = true;
    public bool damagedByBeams   = true;
    public bool damagedByBombs   = true;

    public bool     spawnCyclicPickup = false;
    public PickUp[] spawnSpecificPickup;

    public SoundFx destroyedSounds = null;

    private bool flashing = false;
    private float flashTimer = 0;
    private SpriteRenderer spriteRenderer = null;

    public bool shipExplosion = false;
    public bool smallExplosion = false;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy") &
                    ~LayerMask.GetMask("GroundEnemy") &
                    ~LayerMask.GetMask("EnemyBullets");
        if (polygon)
        {
            polyCollider = GetComponent<Collider2D>();
            Debug.Assert(polyCollider);
        }
        else
            halfExtent = new Vector3(radiusOrWidth / 2, height / 2, 1);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (destroyed) return;

        if (flashing)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                spriteRenderer.material.SetColor("_OverBright", Color.black);
                flashing = false;
            }
        }

        int maxColliders = 10;
        Collider2D[] hits = new Collider2D[maxColliders];
        int noOfHits = 0;
        if (box)
        {
            float angle = transform.eulerAngles.z;
            noOfHits = Physics2D.OverlapBoxNonAlloc(transform.position, halfExtent, angle, hits, layerMask);
        }
        else if (polygon)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(layerMask);
            contactFilter.useLayerMask = true;
            noOfHits = Physics2D.OverlapCollider(polyCollider, contactFilter, hits);
        }
        else
        {
            noOfHits = Physics2D.OverlapCircleNonAlloc(transform.position, radiusOrWidth, hits, layerMask);
        }

        if (noOfHits>0)
        {
            for (int h=0; h<noOfHits; h++)
            {
                if (damagedByBullets)
                {
                    Bullet b = hits[h].GetComponent<Bullet>();
                    if (b != null)
                    {
                        TakeDamage(1, b.playerIndex);
                        GameManager.instance.bulletManager.DeactivateBullet(b.index);
                        FlashAndSpark(b.transform.position);
                    }
                }
                if (damagedByBombs)
                {
                    Bomb bomb = hits[h].GetComponent<Bomb>();
                    if (bomb != null)
                    {
                        TakeDamage(bomb.power, bomb.playerIndex);
                        FlashAndSpark(transform.position);
                    }
                }
                
            }
        }
    }

    public void TakeDamage(int amount, byte fromPlayer)
    {
        if (destroyed) return;

        ScoreManager.instance.ShootableHit(fromPlayer, hitScore);
        health -= amount;

        EnemyPart part = GetComponent<EnemyPart>();
        if (part)
        {
            if (health <= damageHealth)
                part.Damaged(true);
            else
                part.Damaged(false);
        }
        if (health <= 0) // destroyed
        {
            destroyed = true;
            if (part)
                part.Destroyed(fromPlayer);

            if (destroyedSounds)
                destroyedSounds.Play();

            if (fromPlayer < 2)
            {
                ScoreManager.instance.ShootableDestroyed(fromPlayer, destroyScore);

                GameManager.instance.playerDatas[fromPlayer].chain++;
                ScoreManager.instance.UpdateChainMultiplier(fromPlayer);
                GameManager.instance.playerDatas[fromPlayer].chainTimer = PlayerData.MAX_CHAIN_TIMER;
            }
            Vector2 pos = transform.position;
            if (spawnCyclicPickup)
            {
                PickUp spawn = GameManager.instance.GetNextDrop();
                GameManager.instance.SpawnPickup(spawn, pos);
            }

            foreach (PickUp pickup in spawnSpecificPickup)
            {
                GameManager.instance.SpawnPickup(pickup, pos);
            }

            if (smallExplosion)
                EffectSystem.instance.SpawnSmallExplosion(transform.position);
            if (shipExplosion)
                EffectSystem.instance.SpawnShipExplosion(transform.position);

            if (remainDestroyed)
                destroyed = true;
            else
                gameObject.SetActive(false);
        }
    }

    private void FlashAndSpark(Vector3 position)
    {
        EffectSystem.instance.SpawnSparks(position);

        if (flashing) return;
        flashing = true;
        flashTimer = 0.01f;
        spriteRenderer.material.SetColor("_OverBright", Color.white);
    }
}
