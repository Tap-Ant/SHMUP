using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    public AudioClip selectSound = null;
    public AudioClip submitSound = null;
    public void OnSelect(BaseEventData eventData)
    {
        if (selectSound != null)
        {
            AudioManager.instance.PlaySfx(selectSound);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (submitSound != null)
        {
            AudioManager.instance.PlaySfx(submitSound);
        }
    }
}
