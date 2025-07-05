using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeam : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public float        beamWidth    = 10;
    private int         layerMask    = -1;
    public GameObject   beamFlash    = null;
    public GameObject   endPoint     = null;

    private bool        firing           = false;
    private bool        charging         = false;
    private const int   FULL_CHARGE_TIME = 60;
    private int         chargeTimer      = FULL_CHARGE_TIME;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("EnemyBullets") & ~LayerMask.GetMask("Enemy");
    }

    public void Fire()
    {
        if (!firing)
        {
            firing = true;
            charging = true;
            UpdateBeam();
            gameObject.SetActive(true);
        }   
    }

    public void StopFiring()
    {
        firing = false;
        charging = false;
        gameObject.SetActive(false);
        if (beamFlash) beamFlash.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (firing)
            UpdateBeam();
    }

    void UpdateBeam()
    {
        if (!charging)
        {
            int maxColliders = 20;
            Collider2D[] hits = new Collider2D[maxColliders];

            Vector2 center = Vector2.Lerp(transform.position, endPoint.transform.position, 0.5f);
            Vector2 halfSize = new Vector2(beamWidth * 0.5f, (endPoint.transform.position - transform.position).magnitude * 0.5f);
            int noOfHits = Physics2D.OverlapBoxNonAlloc(center, halfSize, transform.eulerAngles.z, hits, layerMask);

            for (int h = 0; h < noOfHits; h++)
            {
                Craft craft = hits[h].GetComponent<Craft>();
                if (craft)
                    craft.Hit();
            }

            // Update visuals
            lineRenderer.startWidth = beamWidth;
            lineRenderer.endWidth = beamWidth;
        }
        else
        {
            // Update visuals
            lineRenderer.startWidth = 1;
            lineRenderer.endWidth = 1;

            // Update timer
            chargeTimer--;
            if (chargeTimer <= 0)
            {
                charging = false;
                chargeTimer = FULL_CHARGE_TIME;

                if (beamFlash) beamFlash.SetActive(true);
            }
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint.transform.position);
    }
}
