using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public float beamWidth = 20;
    public Craft craft = null;
    private int layerMask = -1;
    public byte playerIndex = 2;

    public GameObject beamFlash = null;
    public GameObject[] beamHits = new GameObject[5];

    const int MINIMUMCHARGE = 10;

    public AudioSource audioSource = null;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("PlayerBullets") & ~LayerMask.GetMask("Player");
    }

    public void Fire()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        if (!craftData.beamFiring)
        {
            if (craftData.beamCharge > MINIMUMCHARGE)
            {
                craftData.beamFiring = true;
                craftData.beamTimer = craftData.beamCharge;
                craftData.beamCharge = 0;
                UpdateBeam();
                float scale = beamWidth / 30f;
                beamFlash.transform.localScale = new Vector3(scale, scale, 1);
                gameObject.SetActive(true);
                beamFlash.SetActive(true);
                if (audioSource) audioSource.Play();
            }
            else
            {
                UpdateBeam();
            }
        }   
    }

    private void FixedUpdate()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        if (craftData.beamFiring)
            UpdateBeam();
    }

    void HideHits()
    {
        for (int h = 0; h < 5; h++)
        {
            beamHits[h].SetActive(false);
        }
    }

    void UpdateBeam()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        if (craftData.beamTimer > 0) craftData.beamTimer--;
        if (craftData.beamTimer == 0)
        {
            if (audioSource) audioSource.Stop();
            craftData.beamFiring = false;
            HideHits();
            gameObject.SetActive(false);
            beamFlash.SetActive(false);

            return;
        }

        float scale = beamWidth / 30f;
        beamFlash.transform.localScale = new Vector3(scale, scale, 1);

        float topY = 180;
        if (GameManager.instance && GameManager.instance.progressWindow)
            topY += GameManager.instance.progressWindow.transform.position.y;

        int maxColliders = 20;
        Collider2D[] hits = new Collider2D[maxColliders];
        Vector2 halfSize = new Vector2(beamWidth * 0.5f, (topY - craft.transform.position.y) * 0.5f);
        float middleY = (craft.transform.position.y + topY) * 0.5f;
        Vector3 center = new Vector3(craft.transform.position.x, middleY, 0);
        int noOfHits = Physics2D.OverlapBoxNonAlloc(center, halfSize, 0, hits, layerMask);
        float lowest = topY;
        Shootable lowestShootable = null;
        Collider2D lowestCollider = null;
        const int MAX_RAY_HITS = 10;

        if (noOfHits > 0)
        {
            // Find lowest hit
            for (int hit=0; hit<noOfHits; hit++)
            {
                Shootable shootable = hits[hit].GetComponent<Shootable>();
                if (shootable && shootable.damagedByBeams)
                {
                    RaycastHit2D[] hitInfo = new RaycastHit2D[MAX_RAY_HITS];
                    Vector2 ray = Vector3.up;
                    float height = topY - craft.transform.position.y;
                    if (hits[hit].Raycast(ray, hitInfo, height) > 0)
                    {
                        if (hitInfo[0].point.y < lowest)
                        {
                            lowest = hitInfo[0].point.y;
                            lowestShootable = hits[hit].GetComponent<Shootable>();
                            lowestCollider = hits[hit];
                        }
                    }
                }
            }

            // Find hits on Collider
            if (lowestShootable != null)
            {
                // Fire 5 rays to find each hit
                Vector3 start = craft.transform.position;
                start.x -= (beamWidth / 5);

                for (int h=0; h<5;  h++)
                {
                    RaycastHit2D[] hitInfo = new RaycastHit2D[MAX_RAY_HITS];
                    Vector2 ray = Vector3.up;
                    if (lowestCollider.Raycast(ray, hitInfo, 360) > 0)
                    {
                        Vector3 pos = hitInfo[0].point;
                        pos.x += Random.Range(-3f, 3f);
                        pos.y += Random.Range(-3f, 3f);
                        beamHits[h].transform.position = pos;
                        beamHits[h].SetActive(true);
                        lowestShootable.TakeDamage(craftData.beamPower+1, playerIndex);
                    }
                    else
                    {
                        beamHits[h].SetActive(false);
                    }
                    start.x += (beamWidth / 5);
                }
            }
            else
            {
                HideHits();
            }
        }
        else
        {
            HideHits();
        }

        // Update visuals
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth   = beamWidth;

        lineRenderer.SetPosition(0, transform.position);
        Vector3 top = transform.position;
        top.y = lowest;
        lineRenderer.SetPosition(1, top);
    }
}
