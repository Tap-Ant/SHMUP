using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTrigger : MonoBehaviour
{
    public UnityEvent eventToTrigger;
    public AudioManager.Tracks musicToTrigger = AudioManager.Tracks.None;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        eventToTrigger.Invoke();
        if (musicToTrigger != AudioManager.Tracks.None)
            AudioManager.instance.PlayMusic(musicToTrigger, true, 0.5f);
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position, collider.size);
        }
    }
}
