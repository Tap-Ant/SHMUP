using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFx : MonoBehaviour
{
    public AudioClip[] sounds;

    public void Play()
    {
        if (AudioManager.instance && sounds.Length > 0)
        {
            int r = Random.Range(0, sounds.Length);
            AudioManager.instance.PlaySfx(sounds[r]);
        }
    }
}
