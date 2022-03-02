using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public string audioName;

    private void OnCollisionEnter(Collision other)
    {
        FindObjectOfType<AudioManager>().Play(audioName);
    }
}
