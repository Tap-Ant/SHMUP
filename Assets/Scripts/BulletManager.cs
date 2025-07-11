using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class BulletManager : MonoBehaviour
{
    public Bullet[] bulletPrefabs;
    public enum BulletType
    {
        Bullet1_Size1,
        Bullet1_Size2,
        Bullet1_Size3,
        Bullet1_Size4,
        Bullet1_Size5,
        Bullet2_Size1,
        Bullet2_Size2,
        Bullet2_Size3,
        Bullet2_Size4,
        Bullet3_Size1,
        Bullet3_Size2,
        Bullet3_Size3,
        Bullet3_Size4,
        Bullet10_Size1,
        Bullet10_Size2,
        Bullet10_Size3,
        Bullet10_Size4,

        MAX_TYPES
    }

    const int MAX_BULLET_PER_TYPE = 500;
    const int MAX_BULLET_COUNT = MAX_BULLET_PER_TYPE * (int)BulletType.MAX_TYPES;
    private Bullet[] bullets = new Bullet[MAX_BULLET_COUNT];
    private NativeArray<BulletData> bulletData;
    private TransformAccessArray bulletTransforms;

    ProcessBulletJob jobProcessor;

    // Start is called before the first frame update
    void Start()
    {
        bulletData       = new NativeArray<BulletData>(MAX_BULLET_COUNT, Allocator.Persistent);
        bulletTransforms = new TransformAccessArray(MAX_BULLET_COUNT);

        int index = 0;
        for (int bulletType = (int)BulletType.Bullet1_Size1; bulletType < (int)BulletType.MAX_TYPES; bulletType++)
        {
            for (int b = 0; b < MAX_BULLET_PER_TYPE; b++)
            {
                Bullet newBullet = Instantiate(bulletPrefabs[bulletType]).GetComponent<Bullet>();
                newBullet.index = index;
                newBullet.gameObject.SetActive(false);
                newBullet.transform.SetParent(transform);
                bullets[index] = newBullet;
                bulletTransforms.Add(bullets[index].transform);
                index++;
            }
        }

        jobProcessor = new ProcessBulletJob { bullets = bulletData };
    }

    private void OnDestroy()
    {
        bulletData.Dispose();
        bulletTransforms.Dispose();
    }

    private int NextFreeBulletIndex(BulletType type)
    {
        int startIndex = (int)type * MAX_BULLET_PER_TYPE;
        for (int b = 0; b < MAX_BULLET_PER_TYPE; b++)
        {
            if (!bulletData[startIndex + b].active)
                return startIndex + b;
        }
        return -1;
    }

    public Bullet SpawnBullet(BulletType type, float x, float y, float dX, float dY, float angle,
                              float dAngle, bool homing, byte playerIndex)
    {
        int bulletIndex = NextFreeBulletIndex(type);
        if (bulletIndex>-1)
        {
            Bullet result = bullets[bulletIndex];
            result.playerIndex = playerIndex;
            bulletData[bulletIndex] = new BulletData(x, y, dX, dY, angle, dAngle, (int)type, true, homing);
            bullets[bulletIndex].gameObject.transform.position = new Vector3(x, y, 0);
            bullets[bulletIndex].gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(dX, dY, 0));
            result.gameObject.SetActive(true);

            SpriteRenderer renderer = bullets[bulletIndex].gameObject.GetComponent<SpriteRenderer>();
            if (renderer)
            {
                if (playerIndex == 0)
                    renderer.color = new Color(0, 0.5f, 1, 1);
                else if (playerIndex == 1)
                    renderer.color = new Color(1, 1, 0, 1);
                else
                    renderer.color = new Color(1, 0, 1, 1);
            }

            return result;
        }
        return null;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance && GameManager.instance.playerCrafts[0])
            jobProcessor.player1Position = GameManager.instance.playerCrafts[0].transform.position;
        else
            jobProcessor.player1Position = new Vector2(-9999, -9999);

        if (GameManager.instance && GameManager.instance.progressWindow)
            jobProcessor.progressY = GameManager.instance.progressWindow.transform.position.y;
        else
            jobProcessor.progressY = 0;
        ProcessBullets();

        for (int b=0; b < MAX_BULLET_COUNT; b++)
        {
            if (!bulletData[b].active)
                bullets[b].gameObject.SetActive(false);
        }
    }

    void ProcessBullets()
    {
        JobHandle handler = jobProcessor.Schedule(bulletTransforms);
        handler.Complete();
    }

    public void DeactivateBullet(int index)
    {
        bullets[index].gameObject.SetActive(false);
        float x = bulletData[index].positionX;
        float y = bulletData[index].positionY;
        float dX = bulletData[index].dX;
        float dY = bulletData[index].dY;
        float angle = bulletData[index].angle;
        float dAngle = bulletData[index].dAngle;
        int type = bulletData[index].type;
        bool homing = bulletData[index].homing;

        bulletData[index] = new BulletData(x, y, dX, dY, angle, dAngle, type, false, homing);
    }

    public struct ProcessBulletJob : IJobParallelForTransform
    {
        public NativeArray<BulletData> bullets;
        public Vector2 player1Position;
        public float progressY;

        public void Execute(int index, TransformAccess transform)
        {
            bool active = bullets[index].active;
            if (!active) return;

            float dX     = bullets[index].dX;
            float dY     = bullets[index].dY;
            float x      = bullets[index].positionX;
            float y      = bullets[index].positionY;
            float angle  = bullets[index].angle;
            float dAngle = bullets[index].dAngle;
            int type     = bullets[index].type;
            bool homing  = bullets[index].homing;

            // Homing
            if (player1Position.x < -1000)
                active = false;
            else if (homing)
            {
                Vector2 velocity = new Vector2(dX, dY);
                float speed = velocity.magnitude;
                Vector2 toPlayer = new Vector2(player1Position.x - x, player1Position.y - y);
                Vector2 newVelocity = Vector2.Lerp(velocity.normalized, toPlayer.normalized, 0.05f).normalized;
                newVelocity *= speed;
                dX = newVelocity.x;
                dY = newVelocity.y;
            }

            // Movement
            x = x + dX;
            y = y + dY;

            // Check for out of bounds
            if (x < -320) active = false;
            if (x >  320) active = false;
            if (y-progressY < -180) active = false;
            if (y-progressY >  180) active = false;

            bullets[index] = new BulletData(x, y, dX, dY, angle, dAngle, type, active, homing);

            if (active)
            {
                Vector3 newPosition = new Vector3(x, y, 0);
                transform.position = newPosition;

                // Facing rotation
                transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(dX, dY, 0));
            }
        }
    }
}
