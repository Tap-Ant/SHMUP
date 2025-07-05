using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Craft : MonoBehaviour
{
    Vector3 newPosition = new Vector3();

    public GameObject AftFlame1;
    public GameObject AftFlame2;
    public GameObject LeftFlame;
    public GameObject RightFlame;
    public GameObject FrontFlame;

    public int playerIndex;

    public CraftConfiguration config;
    public BulletSpawner[] bulletSpawner = new BulletSpawner[5];
    public Option[] options = new Option[4];
    public GameObject[] optionMarkersL1 = new GameObject[4];
    public GameObject[] optionMarkersL2 = new GameObject[4];
    public GameObject[] optionMarkersL3 = new GameObject[4];
    public GameObject[] optionMarkersL4 = new GameObject[4];
    public Beam beam = null;
    public GameObject bombPrefab = null;

    Animator animator;
    int leftBoolID;
    int rightBoolID;

    SpriteRenderer spriteRenderer = null;

    public SoundFx explodingNoise = null;
    public SoundFx bombSound = null;

    int layerMask   = 0;
    int pickUpLayer = 0;

    bool alive = true;
    bool invincible = true;
    int invincibleTimer = 120;
    const int INVINCIBLELENGTH = 120;

    public static int MAX_BEAM_CHARGE = 64;
    const int MAX_LIVES = 5;
    const int MAX_SMALL_BOMBS = 8;
    const int MAX_LARGE_BOMBS = 5;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator);

        leftBoolID = Animator.StringToHash("Left");
        rightBoolID = Animator.StringToHash("Right");

        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(spriteRenderer);
        layerMask = ~LayerMask.GetMask("PlayerBullets") &
                    ~LayerMask.GetMask("PlayerBombs") &
                    ~LayerMask.GetMask("Player") &
                    ~LayerMask.GetMask("GroundEnemy");
        pickUpLayer = LayerMask.NameToLayer("PickUp");

        foreach (BulletSpawner s in bulletSpawner)
        {
            s.playerIndex = (byte)playerIndex;
        }
    }

    void FixedUpdate()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];

        if (InputManager.instance && alive)
        {
            // Chain drop
            if (GameManager.instance.playerDatas[playerIndex].chainTimer > 0)
            {
                GameManager.instance.playerDatas[playerIndex].chainTimer--;
                if (GameManager.instance.playerDatas[playerIndex].chainTimer == 0)
                {
                    GameManager.instance.playerDatas[playerIndex].chain = 0;
                    ScoreManager.instance.UpdateChainMultiplier(playerIndex);
                }
            }

            // Invincible flashing
            if (invincible)
            {
                if (invincibleTimer % 12 < 6)
                    spriteRenderer.material.SetColor("_Overbright", Color.black);
                else
                    spriteRenderer.material.SetColor("_Overbright", Color.white);
                invincibleTimer--;
                if (invincibleTimer <= 0)
                {
                    invincible = false;
                    spriteRenderer.material.SetColor("_Overbright", Color.black);
                }
            }

            // Hit detection
            int maxColliders = 10;
            Collider2D[] hits = new Collider2D[maxColliders];

            // Bullet Hits
            Vector2 halfSize = new Vector2(3f, 4f);
            int noOfHits = Physics2D.OverlapBoxNonAlloc(transform.position, halfSize, 0, hits, layerMask);
            if (noOfHits > 0)
            {
                foreach (Collider2D hit in hits)
                {
                    if (hit)
                    {
                        if (hit.gameObject.layer != pickUpLayer)
                            Hit();
                    }
                }
            }

            // Pickups and Bullet grazing
            halfSize = new Vector2(15f, 20f);
            noOfHits = Physics2D.OverlapBoxNonAlloc(transform.position, halfSize, 0, hits, layerMask);
            if (noOfHits > 0)
            {
                foreach (Collider2D hit in hits)
                {
                    if (hit)
                    {
                        if (hit.gameObject.layer == pickUpLayer)
                            PickUp(hit.GetComponent<PickUp>());
                        else if (craftData.beamCharge < MAX_BEAM_CHARGE)// Bullet graze
                        {
                            craftData.beamCharge++;
                            craftData.beamTimer++;
                        }
                    }
                }
            }

            // Movement
            craftData.positionX += InputManager.instance.playerState[playerIndex].movement.x * config.speed;
            craftData.positionY += InputManager.instance.playerState[playerIndex].movement.y * config.speed;
            
            if (craftData.positionX < -130) craftData.positionX = -130;
            if (craftData.positionX > 130) craftData.positionX = 130;
            if (craftData.positionY < -170) craftData.positionY = -170;
            if (craftData.positionY > 170) craftData.positionY = 170;

            newPosition.x = (int)craftData.positionX;
            if (!GameManager.instance.progressWindow) // for testing
                GameManager.instance.progressWindow = GameObject.FindObjectOfType<LevelProgress>();
            if (GameManager.instance.progressWindow)
                newPosition.y = (int)craftData.positionY + GameManager.instance.progressWindow.transform.position.y;
            else
                newPosition.y = (int)craftData.positionY;
            gameObject.transform.position = newPosition;

            if (InputManager.instance.playerState[playerIndex].up)
            {
                AftFlame1.SetActive(true);
                AftFlame2.SetActive(true);
            }
            else
            {
                AftFlame1.SetActive(false);
                AftFlame2.SetActive(false);
            }
            if (InputManager.instance.playerState[playerIndex].down)
            {
                FrontFlame.SetActive(true);
            }
            else
            {
                FrontFlame.SetActive(false);
            }
            if (InputManager.instance.playerState[playerIndex].left)
            {
                RightFlame.SetActive(true);
                AftFlame1.SetActive(true);
                AftFlame2.SetActive(true);
                animator.SetBool(leftBoolID, true);
            }
            else
            {
                RightFlame.SetActive(false);
                animator.SetBool(leftBoolID, false);
            }
            if (InputManager.instance.playerState[playerIndex].right)
            {
                LeftFlame.SetActive(true);
                AftFlame1.SetActive(true);
                AftFlame2.SetActive(true);
                animator.SetBool(rightBoolID, true);
            }
            else
            {
                LeftFlame.SetActive(false);
                animator.SetBool(rightBoolID, false);
            }

            // Shooting bullets
            if (InputManager.instance.playerState[playerIndex].shoot)
            {
                // Shoot
                ShotConfiguration shotConfig = config.shotLevel[craftData.shotPower];
                for (int s=0; s<5; s++)
                {
                    bulletSpawner[s].Shoot(shotConfig.spawnerSizes[s]);
                }

                // Options
                for (int o=0; o<craftData.noOfEnabledOptions; o++)
                {
                    if (options[o])
                        options[o].Shoot();
                }
                
            }

            // Options layout
            if (!InputManager.instance.playerPrevState[playerIndex].options
                && InputManager.instance.playerState[playerIndex].options)
            {
                craftData.optionLayout++;
                if (craftData.optionLayout > 3)
                    craftData.optionLayout = 0;
                SetOptionLayout(craftData.optionLayout);
            }

            // Beam
            if (InputManager.instance.playerState[playerIndex].beam)
            {
                beam.Fire();
            }

            // Bomb
            if (!InputManager.instance.playerPrevState[playerIndex].bomb
                && InputManager.instance.playerState[playerIndex].bomb)
            {
                FireBomb();
            }
        }
    }

    public void SetInvincible()
    {
        invincible = true;
        invincibleTimer = INVINCIBLELENGTH;
    }

    public void PickUp(PickUp pickUp)
    {
        if (pickUp)
        {
            CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
            pickUp.ProcessPickUp(playerIndex, craftData);
        }
    }

    public void Hit()
    {
        if (!invincible && !GameManager.instance.gameSession.invincible)
            Explode();
    }

    public void Explode()
    {
        alive = false;
        GameManager.instance.playerDatas[playerIndex].lives--;
        StartCoroutine(Exploding());

        if (explodingNoise) explodingNoise.Play();
    }

    IEnumerator Exploding()
    {
        Color col = Color.white;
        for (float redness = 0; redness <= 1; redness += 0.3f)
        {
            col.g = 1 - redness;
            col.b = 1 - redness;
            spriteRenderer.color = col;
            yield return new WaitForSeconds(0.1f);
        }

        EffectSystem.instance.CraftExplosion(transform.position);
        Destroy(gameObject);
        GameManager.instance.playerCrafts[playerIndex] = null;

        bool allLivesGone = false;
        if (GameManager.instance.twoPlayer)
        {
            if ((GameManager.instance.playerDatas[0].lives == 0) &&
                (GameManager.instance.playerDatas[1].lives == 0))
            {
                allLivesGone = true;
            }
        }
        else
        {
            if (GameManager.instance.playerDatas[playerIndex].lives == 0)
            {
                allLivesGone = true;
            }
        }

        if (allLivesGone)
        {
            GameOverMenu.instance.GameOver();
        }
        else
        {
            // Eject powerups, respawn
            CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
            int noOfOptionsToRespawn = craftData.noOfEnabledOptions - 1;
            int noOfPowerupsToRespawn = craftData.shotPower - 1;
            int noOfBeamupsToRespawn = craftData.beamPower - 1;
            GameManager.instance.ResetState(playerIndex);

            for (int o = 0; o < noOfOptionsToRespawn; o++)
            {
                PickUp pickup = GameManager.instance.SpawnPickup(GameManager.instance.option, transform.position);
                pickup.transform.position += new Vector3(UnityEngine.Random.Range(-120, 120), UnityEngine.Random.Range(0, 120), 0);
            }

            for (int p = 0; p < noOfPowerupsToRespawn; p++)
            {
                PickUp pickup = GameManager.instance.SpawnPickup(GameManager.instance.powerup, transform.position);
                pickup.transform.position += new Vector3(UnityEngine.Random.Range(-120, 120), UnityEngine.Random.Range(0, 120), 0);
            }

            for (int b = 0; b < noOfPowerupsToRespawn; b++)
            {
                PickUp pickup = GameManager.instance.SpawnPickup(GameManager.instance.beamup, transform.position);
                pickup.transform.position += new Vector3(UnityEngine.Random.Range(-120, 120), UnityEngine.Random.Range(0, 120), 0);
            }

            if (GameManager.instance.playerDatas[playerIndex].lives > 0)
                GameManager.instance.DelayedRespawn(playerIndex);
        }

        yield return null;
    }

    public void AddOption(int surplusValue)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        if (craftData.noOfEnabledOptions<4)
        {
            options[craftData.noOfEnabledOptions].gameObject.SetActive(true);
            craftData.noOfEnabledOptions++;
        }
        else
        {
            ScoreManager.instance.PickupCollected(playerIndex, surplusValue);
        }
    }

    public void SetOptionLayout(int layoutIndex)
    {
        Debug.Assert(layoutIndex < 4);
        
        for (int o=0; o<4; o++)
        {
            switch(layoutIndex)
            {
                case 0:
                    options[o].gameObject.transform.position = optionMarkersL1[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL1[o].transform.rotation;
                    break;
                case 1:
                    options[o].gameObject.transform.position = optionMarkersL2[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL2[o].transform.rotation;
                    break;
                case 2:
                    options[o].gameObject.transform.position = optionMarkersL3[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL3[o].transform.rotation;
                    break;
                case 3:
                    options[o].gameObject.transform.position = optionMarkersL4[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarkersL4[o].transform.rotation;
                    break;
            }
        }
    }

    public void IncreaseBeamStrength(int surplusValue)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        if (craftData.beamPower<5)
        {
            craftData.beamPower++;
            UpdateBeam();
        }
        else
        {
            ScoreManager.instance.PickupCollected(playerIndex, surplusValue);
        }
    }

    void UpdateBeam()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        beam.beamWidth = (craftData.beamPower + 2) * 8f;
    }

    void FireBomb()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        if (craftData.smallBombs > 0)
        {
            craftData.smallBombs--;
            Vector3 pos = transform.position;
            pos.y += 100;
            if (bombSound) bombSound.Play();
            Bomb bomb = Instantiate(bombPrefab, pos, Quaternion.identity).GetComponent<Bomb>();
            if (bomb)
                bomb.playerIndex = (byte)playerIndex;
        }
    }

    public void PowerUp(byte powerLevel, int surplusValue)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        craftData.shotPower += powerLevel;
        if (craftData.shotPower > 8)
        {
            ScoreManager.instance.PickupCollected(playerIndex, surplusValue);
            craftData.shotPower = 8;
        }
    }

    public void IncreaseScore(int value)
    {
        GameManager.instance.playerDatas[playerIndex].score += value;
        GameManager.instance.playerDatas[playerIndex].stageScore += value;
    }

    public void OneUp(int surplusValue)
    {
        GameManager.instance.playerDatas[playerIndex].lives++;
        if (GameManager.instance.playerDatas[playerIndex].lives > MAX_LIVES)
        {
            ScoreManager.instance.PickupCollected(playerIndex, surplusValue);
            GameManager.instance.playerDatas[playerIndex].lives = MAX_LIVES;
        }
    }

    public void AddBomb(int power, int surplusValue)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIndex];
        if (power == 1)
        {
            craftData.smallBombs++;
            if (craftData.smallBombs > MAX_SMALL_BOMBS)
            {
                craftData.smallBombs = MAX_SMALL_BOMBS;
                ScoreManager.instance.PickupCollected(playerIndex, surplusValue);
            }
        }
        else if (power == 2)
        {
            craftData.largeBombs++;
            if (craftData.largeBombs > MAX_LARGE_BOMBS)
            {
                craftData.smallBombs = MAX_LARGE_BOMBS;
                ScoreManager.instance.PickupCollected(playerIndex, surplusValue);
            }
        }
        else
            Debug.LogError("Invalid bomb pickup!");
    }

    public void AddMedal(int level, int value)
    {
        ScoreManager.instance.MedalCollected(playerIndex, value);
    }
}

[Serializable]
public class CraftData
{
    public float positionX;
    public float positionY;

    public byte shotPower;

    public byte noOfEnabledOptions;
    public byte optionLayout;

    public bool beamFiring;
    public byte beamPower;  // power setting & width
    public byte beamCharge; // picked up charge (upgradeable)
    public byte beamTimer;  // current charge level (how much beam left)

    public byte smallBombs;
    public byte largeBombs;

    public void Save(BinaryWriter writer)
    {
        writer.Write(shotPower);
        writer.Write(noOfEnabledOptions);
        writer.Write(optionLayout);
        writer.Write(beamPower);
        writer.Write(beamCharge);
        writer.Write(smallBombs);
        writer.Write(largeBombs);
    }

    public void Load(BinaryReader reader)
    {
        shotPower = reader.ReadByte();
        noOfEnabledOptions = reader.ReadByte();
        optionLayout = reader.ReadByte();
        beamPower = reader.ReadByte();
        beamCharge = reader.ReadByte();
        smallBombs = reader.ReadByte();
        largeBombs = reader.ReadByte();
    }
}
