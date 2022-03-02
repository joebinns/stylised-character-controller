using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    public string audioName;

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<AudioManager>().Play(audioName);
    }
}
