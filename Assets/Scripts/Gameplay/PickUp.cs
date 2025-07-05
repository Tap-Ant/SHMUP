using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PickUp : MonoBehaviour
{
    public enum PickupType
    {
        INVALID,

        Bomb,
        Coin,
        PowerUp,
        BeamUp,
        Options,
        Medal,
        Secret,
        Lives,

        NOOFPICKUPTYPES
    };

    public PickUpConfig config;
    public Vector2 position;
    public Vector2 velocity;
    public SoundFx sounds = null;

    private void OnEnable()
    {
        position = transform.position;
        velocity.x = Random.Range(-4, 4);
        velocity.y = Random.Range(-4, 4);
    }

    private void FixedUpdate()
    {
        // Move
        position += velocity;
        velocity /= 1.3f;
        position.y -= config.fallSpeed;
        if (GameManager.instance && GameManager.instance.progressWindow)
        {
            float posY = position.y - GameManager.instance.progressWindow.transform.position.y;
            if (posY < -180) // offscreen
            {
                GameManager.instance.PickUpFallOffScreen(this);
                Destroy(gameObject);
                return;
            }
        }
        transform.position = position;
    }

    public void ProcessPickUp(int playerIndex, CraftData craftData)
    {
        if (sounds)
            sounds.Play();
        switch (config.type)
        {
            case PickupType.Coin:
                {
                    ScoreManager.instance.PickupCollected(playerIndex, config.coinValue);
                    break;
                }
            case PickupType.PowerUp:
                {
                    GameManager.instance.playerCrafts[playerIndex].PowerUp((byte)config.powerLevel, config.surplusValue);
                    break;
                }
            case PickupType.Lives:
                {
                    GameManager.instance.playerCrafts[playerIndex].OneUp(config.surplusValue);
                    break;
                }
            case PickupType.Secret:
                {
                    ScoreManager.instance.PickupCollected(playerIndex, config.coinValue);
                    break;
                }
            case PickupType.BeamUp:
                {
                    GameManager.instance.playerCrafts[playerIndex].IncreaseBeamStrength(config.surplusValue);
                    break;
                }
            case PickupType.Options:
                {
                    GameManager.instance.playerCrafts[playerIndex].AddOption(config.surplusValue);
                    break;
                }
            case PickupType.Bomb:
                {
                    GameManager.instance.playerCrafts[playerIndex].AddBomb(config.bombPower, config.surplusValue);
                    break;
                }
            case PickupType.Medal:
                {
                    GameManager.instance.playerCrafts[playerIndex].AddMedal(config.medalLevel,
                                                                            config.medalValue);
                    break;
                }
            default:
                {
                    Debug.LogError("Unprocessed config type: "+config.type);
                    break;
                }
        };
        Destroy(gameObject);
    }
}
